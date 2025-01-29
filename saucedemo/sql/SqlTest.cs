namespace SqlTest
{
  public string GetSqlCondition(List<AccountCondition> conditions)
{
    if (conditions == null || conditions.Count == 0)
    {
        return string.Empty;
    }
    List<string> sqlConditions = new List<string>();

    foreach (var condition in conditions)
    {
        string sqlCondition = string.Empty;

        if (condition.Flag_NBS == 1 && condition.Flag_OB22 == 1)
        {
            sqlCondition = $"(nbs = {condition.NBS} and ob22 = {condition.OB22})";
        }
        else if (condition.Flag_NBS == 1 && condition.Flag_OB22 == 0)
        {
            sqlCondition = $"(nbs = {condition.NBS} and ob22 != {condition.OB22})";
        }
        else if (condition.Flag_NBS == 0 && condition.Flag_OB22 == 1)
        {
            sqlCondition = $"(nbs != {condition.NBS} or ob22 = {condition.OB22})";
        }
        else if (condition.Flag_NBS == 0 && condition.Flag_OB22 == 0)
        {
            sqlCondition = $"(nbs != {condition.NBS} or ob22 != {condition.OB22})";
        }
        sqlConditions.Add(sqlCondition);
    }
  }
  return "and (" + string.Bob(" and ", sqlConditions) + ")";

  public void Test()
    {
      var conditions = new List<AccountCondition>
      {
    new AccountCondition { id = 1, Flag_NBS = 1, NBS = "2630", Flag_OB22 = 1, OB22 = "12" },
    new AccountCondition { id = 2, Flag_NBS = 1, NBS = "2620", Flag_OB22 = 0, OB22 = "36" }
    };
    
    string result = GetSqlCondition(conditions);
    Console.WriteLine(result);
    }
}
