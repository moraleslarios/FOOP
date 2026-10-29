// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Helpers;

/// <summary>
/// Tests de la epica 01 de la 2.1.1: constantes, literales de mensaje y nomenclatura.
/// Verifican que los nombres <c>PascalCase</c> y los puentes <c>[Obsolete]</c> conviven
/// con el mismo valor y con un mecanismo de declaracion homogeneo.
/// </summary>
public class ConstantsTests
{

    #region US-01 · Erratas de los literales de mensaje

    [Fact]
    public void MlError_SinMensaje_UsaMensajePorDefecto()
    {
        MlError error = new("   ");

        error.Message.Should().Be(Constants.DefaultErrorMessage);
    }

    [Fact]
    public void Constants_DefaultErrorMessage_ComillasBalanceadas()
    {
        var simples = Constants.DefaultErrorMessage.Count(c => c == '\'');

        (simples % 2).Should().Be(0, "las comillas simples deben estar balanceadas");
    }

    [Fact]
    public void EnsureFpMessages_DefaultExceptionError_TieneEspacioTrasPunto()
    {
        var mensaje = EnsureFpMessages.DefaultExceptionError(new InvalidOperationException("boom"));

        mensaje.Should().NotContain(".More");
    }

    #endregion

    #region US-04 · DefaultExceptionError en EnsureFpMessages

    [Fact]
    public void EnsureFpMessages_DefaultExceptionError_FormatoCorrecto()
    {
        var ex = new InvalidOperationException("boom");

        var mensaje = EnsureFpMessages.DefaultExceptionError(ex);

        mensaje.Should().Be("An error occurred while executing the function. Error: boom. More info in 'Ex' details.");
    }

    [Fact]
    public void EnsureFpMessages_DefaultExceptionError_ExcepcionNula_LanzaArgumentNullException()
    {
        Action act = () => EnsureFpMessages.DefaultExceptionError(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("ex");
    }

#pragma warning disable CS0618 // Se comprueba a proposito el puente [Obsolete].

    [Fact]
    public void Constants_DefaultExErrorMessage_DelegaEnEnsureFpMessages()
    {
        var ex = new InvalidOperationException("boom");

        Constants.DEFAULT_EX_ERROR_MESSAGE(ex).Should().Be(EnsureFpMessages.DefaultExceptionError(ex));
    }

    #endregion

    #region US-05 · Gemelos PascalCase

    [Fact]
    public void Constants_NombresNuevos_CoincidenConLosAntiguos()
    {
        Constants.DefaultErrorMessage .Should().Be(Constants.DEFAULT_ERROR_MESSAGE);
        Constants.NotFoundKey         .Should().Be(Constants.NOT_FOUND_KEY);
        Constants.ParameterNameKey    .Should().Be(Constants.PARAM_NAME_KEY);
        Constants.FailedIndexesKey    .Should().Be(Constants.FAILED_INDEXES_KEY);
        Constants.ExpectedKey         .Should().Be(Constants.EXPECTED_KEY);
        Constants.ExceptionMetadataKey.Should().Be(Constants.EX_DESC_KEY);
        Constants.ValueMetadataKey    .Should().Be(Constants.VALUE_KEY);
    }

#pragma warning restore CS0618

    [Fact]
    public void Constants_ClavesDeMetadatos_MecanismoHomogeneo()
    {
        string[] nombres =
        [
            nameof(Constants.DefaultErrorMessage),
            nameof(Constants.ExceptionMetadataKey),
            nameof(Constants.ValueMetadataKey),
            nameof(Constants.NotFoundKey),
            nameof(Constants.ParameterNameKey),
            nameof(Constants.FailedIndexesKey),
            nameof(Constants.ExpectedKey)
        ];

        foreach (var nombre in nombres)
        {
            var campo = typeof(Constants).GetField(nombre, BindingFlags.Public | BindingFlags.Static);

            campo.Should().NotBeNull($"'{nombre}' debe declararse como campo static readonly");
            campo!.IsInitOnly.Should().BeTrue($"'{nombre}' debe ser readonly");
            campo .IsLiteral .Should().BeFalse($"'{nombre}' no debe ser const: se hornearia en el consumidor");
        }
    }

    [Fact]
    public void Constants_MiembrosConGuionBajo_EstanObsoletos()
    {
        string[] antiguos =
        [
            "DEFAULT_ERROR_MESSAGE",
            "NOT_FOUND_KEY",
            "PARAM_NAME_KEY",
            "FAILED_INDEXES_KEY",
            "EXPECTED_KEY",
            "EX_DESC_KEY",
            "VALUE_KEY",
            "DEFAULT_EX_ERROR_MESSAGE"
        ];

        foreach (var nombre in antiguos)
        {
            MemberInfo? miembro = typeof(Constants).GetProperty(nombre, BindingFlags.Public | BindingFlags.Static)
                               ?? (MemberInfo?)typeof(Constants).GetMethod(nombre, BindingFlags.Public | BindingFlags.Static);

            miembro.Should().NotBeNull($"'{nombre}' debe seguir existiendo como puente");
            miembro!.GetCustomAttribute<ObsoleteAttribute>()
                    .Should().NotBeNull($"'{nombre}' debe estar marcado como [Obsolete]");
        }
    }

    [Fact]
    public void Constants_NoDeclaraMetodosPropios()
    {
        var metodos = typeof(Constants).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                                       .Where(m => !m.IsSpecialName)
                                       .Where(m => m.GetCustomAttribute<ObsoleteAttribute>() is null)
                                       .ToArray();

        metodos.Should().BeEmpty("Constants solo debe declarar valores, nunca logica");
    }

    #endregion

    #region US-06 · Literales centralizados

    [Fact]
    public void MlResult_ToString_ValorNulo_UsaRepresentacionCentralizada()
    {
        MlResult<string?> result = MlResult<string?>.Valid(null);

        result.ToString().Should().Be(EnsureFpMessages.NoValueRepresentation);
    }

    [Fact]
    public void MlErrorDetails_ToString_CabecerasCentralizadas()
    {
        var details = MlErrorDetails.FromErrorDetails("fallo", "clave", "valor");

        var texto = details.ToString();

        texto.Should().Contain(EnsureFpMessages.ErrorsSectionHeader);
        texto.Should().Contain(EnsureFpMessages.MetadataSectionHeader);
    }

    #endregion

}
