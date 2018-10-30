using AutoMapper;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceDemo.Service.Infrastructure
{
    public class MapConfiguration
    {
        public static void AddProfile(IMapperConfigurationExpression cfg)
        {
            bool IsToRepeatedField(PropertyMap pm)
            {
                if (pm.DestinationPropertyType.IsConstructedGenericType)
                {
                    var destGenericBase = pm.DestinationPropertyType.GetGenericTypeDefinition();
                    return destGenericBase == typeof(RepeatedField<>);
                }
                return false;
            }

            cfg.ForAllPropertyMaps(IsToRepeatedField, (propertyMap, opts) => opts.UseDestinationValue());

            cfg.CreateMap<string, string>().ConvertUsing((src, dest) => src ?? string.Empty);
        }
    }
}
