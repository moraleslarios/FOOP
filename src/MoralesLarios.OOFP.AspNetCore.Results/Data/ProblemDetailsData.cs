namespace MoralesLarios.OOFP.AspNetCore.Results.Data;

/// <summary>
/// Representa <c>ProblemDetailsData</c>.
/// </summary>
/// <param name="Status">Valor de <c>Status</c>.</param>
/// <param name="Title">Valor de <c>Title</c>.</param>
/// <param name="Detail">Valor de <c>Detail</c>.</param>
/// <param name="Type">Valor de <c>Type</c>.</param>
/// <param name="Errors">Valor de <c>Errors</c>.</param>
/// <param name="StatusCode">Valor de <c>StatusCode</c>.</param>
public record ProblemDetailsData(int                        Status,
                                 string                     Title,
                                 string                     Detail,
                                 string                     Type,
                                 Dictionary<string, object> Errors,
                                 int                        StatusCode);
