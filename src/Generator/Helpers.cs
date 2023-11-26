namespace SimpleAPI.Generator;

public static class Helpers
{
    public const string StronglyTypeAttributeName = "HasStronglyTypedID";
    public const string EntityConversionAttributeName = "HasEntityIDConversion";

    public const string StronglyTypeAttributeNameCode =
        $$$"""
        namespace SimpleAPI.Core
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public class {{{StronglyTypeAttributeName}}} : System.Attribute { }
            
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public class {{{EntityConversionAttributeName}}} : System.Attribute { }
        }
        """;
}