namespace MyBudget.Dtos.CategoryDtos
{
    public record CategoryDto
    (
        Guid Id,
        Guid UserId,
        string Name
    );
}
