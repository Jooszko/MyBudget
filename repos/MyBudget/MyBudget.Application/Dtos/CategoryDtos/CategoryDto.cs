namespace MyBudget.Application.Dtos.CategoryDtos
{
    public record CategoryDto
    (
        Guid Id,
        Guid UserId,
        string Name
    );
}
