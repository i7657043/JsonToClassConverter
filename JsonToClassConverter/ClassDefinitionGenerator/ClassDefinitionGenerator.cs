public class ClassDefinitionGenerator
{
    public List<CSharpClass> GenerateClassDefinitions(JsonClass model, List<CSharpClass> classDefinitions)
    {
        CSharpClass classDefinition = new CSharpClass(model.Name);

        foreach (JsonField field in model.Fields)
            classDefinition.Fields.Add(
                new CSharpField(
                    field.Name,
                    field.Type.Name,
                    field.IsArray));

        foreach (JsonClass child in model.Children)
            classDefinition.Fields.Add(new CSharpField(child.Name, child.Name, child.IsArray));

        CSharpClass? duplicate = GetDuplicate(classDefinitions, classDefinition);

        if (duplicate == null)
            classDefinitions.Add(classDefinition);
        else
        {

            //As we aren't writing this new class definition to output (because its a dupe), we need to modify any references to it in other classes
            UpdateTypeReferences(classDefinitions, classDefinition, duplicate);

            //List<CSharpField> fields = classDefinitions.First(field => field. == "Nullable`1").Fields;`
        }            

        foreach (JsonClass child in model.Children)
            GenerateClassDefinitions(child, classDefinitions);

        return classDefinitions;
    }

    private void UpdateTypeReferences(List<CSharpClass> classDefinitions, CSharpClass classDefinition, CSharpClass duplicate)
    {
        classDefinitions.ForEach(classDef =>
        {
            classDef.Fields.ForEach(field =>
            {
                if (field.Type == classDefinition.Name)
                    field.Type = duplicate.Name;

                //we arent adding this class definition to the class defs output because its a duplicate, but sometimes we need the types as the first
                //instnace we added to class defs had null values, so we replace them here
                if (field.Type == "Nullable`1")
                    field.Type = classDefinition.Fields.First(classDefField => field.Name == classDefField.Name).Type;                
            });
        });
    }

    private CSharpClass? GetDuplicate(List<CSharpClass> classDefinitions, CSharpClass classDefinition) =>
        classDefinitions.FirstOrDefault(
            classDef => String.Join(string.Empty, classDef.Fields.Select(x => x.Name)) ==
            String.Join(string.Empty, classDefinition.Fields.Select(x => x.Name)));
}