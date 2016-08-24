﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData.Builder;
using System.Runtime.Serialization;

namespace GenericODataWebApi.OData
{
    static class TempAliasingStuff
    {
        public static IEnumerable<MemberInfo> GetMetadataMembers(this Type type)
        {
            var metadataAttr = type.GetCustomAttributes<MetadataTypeAttribute>().FirstOrDefault();

            IEnumerable<MemberInfo> metadataMembers = metadataAttr?.MetadataClassType.GetProperties();
            metadataMembers = metadataMembers?.Concat(metadataAttr.MetadataClassType.GetFields());

            return metadataMembers ?? Enumerable.Empty<MemberInfo>();
        }

        private static void SetupMetadataAliasingOnModelCreating(ODataConventionModelBuilder builder)
        {
            foreach (var entitySet in builder.EntitySets)
            {
                SetupAliasingFromMetadata(entitySet);
            }
        }

        private static void SetupAliasingFromMetadata(EntitySetConfiguration entitySet)
        {
            var clrType = entitySet.ClrType;
            var metadataType = clrType.GetCustomAttributes<MetadataTypeAttribute>().FirstOrDefault()?.MetadataClassType;

            if (IsDataContractEnabledOnType(metadataType))
            {
                if (IsDataContractEnabledOnType(clrType))
                {
                    throw new InvalidOperationException(
                        $"Entity '{entitySet.Name}' of type '{clrType.Name}' already has a DataContract attribute, please remove this before you apply a DataContract on the metadata!");
                }

                var excludedProperties = entitySet.EntityType.Properties.ToList();
                var metadataProperties = clrType.GetMetadataMembers();

                foreach (var metadataProperty in metadataProperties)
                {
                    var entityProperty = excludedProperties.FirstOrDefault(pc => pc.Name == metadataProperty.Name);

                    // Property might be part of metadata class, but not entity class.
                    if (entityProperty != null)
                    {
                        var dataMemberAttribute = metadataProperty.GetCustomAttribute<DataMemberAttribute>();

                        if (dataMemberAttribute != null)
                        {
                            excludedProperties.Remove(excludedProperties.Single(p => p.Name == metadataProperty.Name));

                            if (dataMemberAttribute.Name != null)
                            {
                                entityProperty.Name = dataMemberAttribute.Name;
                            }
                        }
                    }
                }

                IgnoreProperties(entitySet.EntityType, excludedProperties.Select(p => p.PropertyInfo));
            }
        }

        private static bool IsDataContractEnabledOnType(Type type)
        {
            return type != null && type.GetTypeInfo().GetCustomAttributes(typeof(DataContractAttribute), inherit: true).Any();
        }

        private static void IgnoreProperties(EntityTypeConfiguration entityType, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                IgnorePropertyOnEntityType(entityType, property);
            }
        }

        private static void IgnorePropertyOnEntityType(EntityTypeConfiguration entityType, PropertyInfo propInfo)
        {
            //This method builds an expression to execute the equivilant of:
            // builder.EntityType<MyEntity>().Ignore<string>(e => e.SomeStringProperty)

            var builder = entityType.ModelBuilder;
            var clrType = entityType.ClrType;

            //MethodInfo for method: builder.EntityType<MyEntity>()
            var entityTypeGetMethodInfo = builder.GetType().GetMethod("EntityType").MakeGenericMethod(clrType);

            //Type returned by above method: EntityTypeConfiguration<MyEntity>
            var desiredEntityTypeConfigurationType = entityTypeGetMethodInfo.ReturnType;

            //MethodInfo for method: .Ignore<string>(*parameters*)
            var ignoreMethodInfo = desiredEntityTypeConfigurationType.GetMethod("Ignore").MakeGenericMethod(propInfo.PropertyType);

            var callEntityTypeGetMethod = Expression.Call(Expression.Constant(builder), entityTypeGetMethodInfo);
            var propAccessorLambda = GetPropretyAccessorExpression(propInfo);

            //Resolves EntityTypeConfiguration<T> on which to call the method from "callEntityTypeGetMethod" expression
            //The "Ignore" method receives an Expression as a parameter, which is "propAccessorLambda" below
            var callIgnoreMethodExpression = Expression.Call(callEntityTypeGetMethod, ignoreMethodInfo, propAccessorLambda);

            var topLevelLambda = Expression.Lambda(callIgnoreMethodExpression);
            topLevelLambda.Compile().DynamicInvoke();
        }

        private static Expression GetPropretyAccessorExpression(PropertyInfo propInfo)
        {
            //This method builds an expression equivilant to:
            // instance => instance.SomeProperty
            var paramExpr = Expression.Parameter(propInfo.DeclaringType);
            var propertyExpr = Expression.Property(paramExpr, propInfo.Name);

            //Does this need to be a Lambda? need to learn more about Expressions
            return Expression.Lambda(propertyExpr, paramExpr);
        }
    }
}
