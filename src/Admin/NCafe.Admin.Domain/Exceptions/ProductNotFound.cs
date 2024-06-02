using NCafe.Core.Exceptions;

namespace NCafe.Admin.Domain.Exceptions;

public class ProductNotFound(Guid id) : DomainException($"Product with id '{id}' was not found.");
