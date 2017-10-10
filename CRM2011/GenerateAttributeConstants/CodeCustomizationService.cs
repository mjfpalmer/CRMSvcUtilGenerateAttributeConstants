// Define REMOVE_PROXY_TYPE_ASSEMBLY_ATTRIBUTE if you plan on compiling the output from
// this CrmSvcUtil extension with the output from the unextended CrmSvcUtil in the same
// assembly (this assembly attribute can only be defined once in the assembly).
#define REMOVE_PROXY_TYPE_ASSEMBLY_ATTRIBUTE

namespace GenerateAttributeConstants
{
  using System;
  using System.CodeDom;
  using System.Collections.Generic;
  using Microsoft.Crm.Services.Utility;

  public sealed class CodeCustomizationService : ICustomizeCodeDomService
  {
    /// <summary>
    /// Replace all properties with attribute constants
    /// Remove all unnecessary class members
    /// </summary>
    public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
    {
      string baseTypes = GetParameter("/basetypes");

      foreach (CodeNamespace codeNamespace in codeUnit.Namespaces)
      {
        foreach (CodeTypeDeclaration codeTypeDeclaration in codeNamespace.Types)
        {
          if (codeTypeDeclaration.IsClass)
          {
            codeTypeDeclaration.CustomAttributes.Clear();

            if (!string.IsNullOrEmpty(baseTypes))
            {
              codeTypeDeclaration.BaseTypes.Clear();
              codeTypeDeclaration.BaseTypes.Add(baseTypes);
            }

            List<string> attributes = new List<string>();
            for (var j = 0; j < codeTypeDeclaration.Members.Count; )
            {
              if (codeTypeDeclaration.Members[j].GetType() == typeof(System.CodeDom.CodeMemberProperty) && codeTypeDeclaration.Members[j].CustomAttributes.Count > 0 && codeTypeDeclaration.Members[j].CustomAttributes[0].AttributeType.BaseType == "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute")
              {
                string attribute = ((System.CodeDom.CodePrimitiveExpression)codeTypeDeclaration.Members[j].CustomAttributes[0].Arguments[0].Value).Value.ToString();

                if (!attributes.Contains(attribute))
                {
                  attributes.Add(attribute);
                  codeTypeDeclaration.Members[j] = new CodeMemberField
                  {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = string.Format("{0}Attribute", attribute),
                    Type = new CodeTypeReference(typeof(System.String)),
                    InitExpression = new CodePrimitiveExpression(((System.CodeDom.CodePrimitiveExpression)codeTypeDeclaration.Members[j].CustomAttributes[0].Arguments[0].Value).Value)
                  };
                  j++;
                }
                else
                {
                  codeTypeDeclaration.Members.RemoveAt(j);
                }
              }
              else
              {
                codeTypeDeclaration.Members.RemoveAt(j);
              }
            }
          }
        }
      }

#if REMOVE_PROXY_TYPE_ASSEMBLY_ATTRIBUTE
      foreach (CodeAttributeDeclaration attribute in codeUnit.AssemblyCustomAttributes)
      {
        if (attribute.AttributeType.BaseType == "Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute")
        {
          codeUnit.AssemblyCustomAttributes.Remove(attribute);
          break;
        }
      }
#endif
    }

    private static string GetParameter(string key)
    {
      string[] args = Environment.GetCommandLineArgs();
      foreach (string arg in args)
      {
        string[] argument = arg.Split(new char[] { ':' }, 2);
        if (argument.Length == 2 && argument[0].ToLowerInvariant() == key.ToLowerInvariant())
        {
          return argument[1].Trim(new char[] { '"' });
        }
      }

      return null;
    }
  }
}