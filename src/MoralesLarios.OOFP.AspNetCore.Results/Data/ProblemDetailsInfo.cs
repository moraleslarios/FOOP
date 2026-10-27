namespace MoralesLarios.OOFP.AspNetCore.Results.Data;

public record ProblemDetailsInfo(int                        Status,
                                 string                     Title,
                                 string                     Detail,
                                 string                     Type,
                                 Dictionary<string, object> Errors,
                                 int                        StatusCode);
