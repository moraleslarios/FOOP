// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0





using FluentValidation.Results;
using System.ComponentModel;

namespace MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var User = new User("", DateTime.Now, "123456", "123456");

        var validator = new UserValidator();

        ValidationResult result = validator.Validate(User);

        foreach (var error in result.Errors)
        {
            
        }


    }
}
