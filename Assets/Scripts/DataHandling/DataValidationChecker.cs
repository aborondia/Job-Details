using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataValidationChecker
{
    public static bool IsDateTimeStringValid(string value)
    {
        DateTime dateTime;

        return DateTime.TryParse(value, out dateTime);
    }
}
