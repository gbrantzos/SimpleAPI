namespace SimpleAPI.Generator;

public static class Helpers
{
    public const string StronglyTypeAttributeName = "HasStronglyTypedID";

    public const string StronglyTypeAttributeNameCode =
        $$"""
        namespace SimpleAPI.Generator
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public class {{StronglyTypeAttributeName}} : System.Attribute { }
        }
        """;
}
