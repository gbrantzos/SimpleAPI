namespace SimpleAPI.Generator;

public static class Helpers
{
    public const string StronglyTypeAttributeName = "StronglyTypedID";

    public const string StronglyTypeAttributeNameCode =
        $$$"""
        namespace SimpleAPI.Core
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public class {{{StronglyTypeAttributeName}}} : System.Attribute { }
        }
        """;
}
