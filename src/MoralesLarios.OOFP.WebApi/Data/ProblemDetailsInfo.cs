namespace MoralesLarios.OOFP.WebApi.Data;

public record ProblemDetailsInfo(int                        Status,
                                 string                     Title,
                                 string                     Detail,
                                 string                     Type,
                                 Dictionary<string, object> Errors,
                                 int                        StatusCode);
