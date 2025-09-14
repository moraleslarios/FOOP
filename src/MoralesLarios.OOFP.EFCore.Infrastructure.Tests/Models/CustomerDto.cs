

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Models;
public class CustomerDto
{
    public int CustomerKey { get; set; }

    public int? GeographyKey { get; set; }

    [StringLength(15)]
    public string CustomerAlternateKey { get; set; } = null!;

    [StringLength(8)]
    public string? Title { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? MiddleName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    public bool? NameStyle { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(1)]
    public string? MaritalStatus { get; set; }

    [StringLength(10)]
    public string? Suffix { get; set; }

    [StringLength(1)]
    public string? Gender { get; set; }

    [StringLength(50)]
    public string? EmailAddress { get; set; }

    [Column(TypeName = "money")]
    public decimal? YearlyIncome { get; set; }

    public byte? TotalChildren { get; set; }

    public byte? NumberChildrenAtHome { get; set; }

    [StringLength(40)]
    public string? EnglishEducation { get; set; }

    [StringLength(40)]
    public string? SpanishEducation { get; set; }

    [StringLength(40)]
    public string? FrenchEducation { get; set; }

    [StringLength(100)]
    public string? EnglishOccupation { get; set; }

    [StringLength(100)]
    public string? SpanishOccupation { get; set; }

    [StringLength(100)]
    public string? FrenchOccupation { get; set; }

    [StringLength(1)]
    public string? HouseOwnerFlag { get; set; }

    public byte? NumberCarsOwned { get; set; }

    [StringLength(120)]
    public string? AddressLine1 { get; set; }

    [StringLength(120)]
    public string? AddressLine2 { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public DateOnly? DateFirstPurchase { get; set; }

    [StringLength(15)]
    public string? CommuteDistance { get; set; }

    //[InverseProperty("CustomerKeyNavigation")]
    //public virtual ICollection<FactInternetSale> FactInternetSales { get; set; } = new List<FactInternetSale>();

    //[InverseProperty("CustomerKeyNavigation")]
    //public virtual ICollection<FactSurveyResponse> FactSurveyResponses { get; set; } = new List<FactSurveyResponse>();

    //[InverseProperty("DimCustomers")]
    //public virtual DimGeography? GeographyKeyNavigation { get; set; }
}
