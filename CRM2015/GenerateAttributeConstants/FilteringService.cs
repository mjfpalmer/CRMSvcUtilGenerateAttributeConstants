using System;
using System.Collections.Generic;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System.Globalization;

namespace GenerateAttributeConstants
{
  public sealed class FilteringService : ICodeWriterFilterService
  {
    private ICodeWriterFilterService DefaultService { get; set; }

    public FilteringService(ICodeWriterFilterService defaultService)
    {
      DefaultService = defaultService;
    }

    public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
    {
      return false;
    }

    public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
    {
      return string.IsNullOrEmpty(attributeMetadata.AttributeOf) && !(attributeMetadata.AttributeType == AttributeTypeCode.Money && attributeMetadata.LogicalName.EndsWith("_base", true, CultureInfo.InvariantCulture));
    }

    public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
    {
      return DefaultService.GenerateEntity(entityMetadata, services);
    }

    public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
    {
      return false;
    }

    public bool GenerateServiceContext(IServiceProvider services)
    {
      return false;
    }

    public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
    {
      return false;
    }
  }
}