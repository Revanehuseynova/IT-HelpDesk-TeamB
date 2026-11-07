using System.ComponentModel.DataAnnotations;
using HelpDesk.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDesk.Web.ViewModels;

public class TicketCreateViewModel
{
    [Required(ErrorMessage = "Başlıq mütləqdir.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Başlıq 5-150 simvol arasında olmalıdır.")]
    [Display(Name = "Başlıq")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Təsvir mütləqdir.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Təsvir 10-2000 simvol arasında olmalıdır.")]
    [Display(Name = "Təsvir")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kateqoriya seçilməlidir.")]
    [Display(Name = "Kateqoriya")]
    public int? CategoryId { get; set; }

    [Display(Name = "Prioritet")]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [BindNever]
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    [BindNever]
    public IEnumerable<SelectListItem> Priorities { get; set; } = new List<SelectListItem>();
}
