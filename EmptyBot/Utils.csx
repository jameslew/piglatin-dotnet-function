using System;

public static string GetEnv(string name)
{
  return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process); 
}