using System;

[Serializable]
public class APINumbers
{
    public string label;
    public int value;
}

[Serializable]
public class NumbersCollection
{
    public APINumbers[] numbers;
}

