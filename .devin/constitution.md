# Constitution for Mavin AI - Core API Clean Additional Service

This constitution defines the fundamental principles and constraints that Mavin AI must follow when working on this .NET API project.

## Core Principles

### 1. Architectural Integrity
- **Never violate Clean Architecture principles**: Dependencies must always flow inward (API → Library → Core)
- **Maintain separation of concerns**: Controllers handle HTTP, Directors handle business logic, Repositories handle data access
- **Preserve existing patterns**: When extending functionality, follow established patterns (Director, Repository, Unit of Work)
- **No cross-layer violations**: Controllers should not directly access repositories or DbContext

### 2. Code Quality Standards
- **Write production-ready code**: Code must be maintainable, testable, and follow best practices
- **Follow existing conventions**: Match the coding style, naming conventions, and patterns already in the codebase
- **No commented-out code**: Remove or properly comment code. Do not leave large blocks of commented code
- **Meaningful names**: Use descriptive names for classes, methods, and variables that explain their purpose

### 3. Testing and Verification
- **Test before deploying**: Always write or update tests when adding new functionality
- **Verify builds**: Ensure `dotnet build` succeeds before considering a task complete
- **Test edge cases**: Consider null values, empty collections, boundary conditions, and error scenarios
- **No broken functionality**: Never break existing features without explicit user approval

### 4. Security and Safety
- **Never expose secrets**: Do not commit API keys, connection strings, or sensitive data
- **Use secure defaults**: Configure security settings appropriately (CORS, authentication, authorization)
- **Validate inputs**: Always validate user input and sanitize data
- **Follow OWASP guidelines**: Implement security best practices for web APIs

### 5. Performance and Scalability
- **Use async/await**: All I/O operations must be asynchronous
- **Consider resource usage**: Be mindful of memory, CPU, and database connection usage
- **Optimize database access**: Use efficient queries, avoid N+1 problems, consider pagination
- **Plan for caching**: When implementing features, consider how caching might improve performance

### 6. Configuration Management
- **Externalize configuration**: Use appsettings.json and environment variables, not hard-coded values
- **Support multiple environments**: Ensure configuration works for Development, Staging, and Production
- **Document configuration**: Add comments for non-obvious configuration settings
- **Use dependency injection**: Register services properly with appropriate lifetimes

## Specific Constraints

### Entity Framework and Database
- **Always use migrations**: Never modify database schema without creating proper EF Core migrations
- **Use DbContext correctly**: Scoped lifetime, proper disposal, no direct access from controllers
- **Enable development features**: Detailed errors and sensitive data logging only in development
- **Seed data appropriately**: Use database seeding for initial data, not hard-coded test data in controllers

### API Design
- **RESTful conventions**: Follow HTTP verb semantics (GET for read, POST for create, PUT/PATCH for update, DELETE for delete)
- **Use DTOs**: Never expose domain entities directly to API layer
- **Proper status codes**: Return appropriate HTTP status codes (200, 201, 400, 404, 500, etc.)
- **Error handling**: Implement consistent error handling with meaningful error messages
- **API documentation**: Update Swagger/OpenAPI documentation when API changes are made

### Dependency Injection
- **Interface-based design**: Always depend on interfaces, not concrete implementations
- **Appropriate lifetimes**: 
  - Singleton for stateless services
  - Scoped for per-request services (repositories, DbContext)
  - Transient for lightweight services
- **No service locator pattern**: Use constructor injection, never manually resolve services
- **Register in DependencyInjection.cs**: All service registrations must be in the designated file

### Logging and Monitoring
- **Use Serilog**: Follow existing logging patterns in the codebase
- **Log appropriately**: Log important events, errors, and performance metrics
- **No sensitive data in logs**: Never log passwords, tokens, or other sensitive information
- **Structured logging**: Use structured logging with meaningful context

## Decision Framework

### When Multiple Approaches Exist
1. **Prefer existing patterns**: Choose the approach that matches existing code patterns
2. **Consider maintainability**: Choose the most maintainable solution over clever but obscure solutions
3. **Ask for clarification**: If uncertain, ask the user rather than making assumptions
4. **Document the decision**: Add comments explaining why a particular approach was chosen

### When Requirements Are Unclear
1. **Ask specific questions**: Do not guess; ask for clarification
2. **Propose options**: Present multiple approaches with trade-offs
3. **Start with simplest solution**: Implement the minimum viable solution first
4. **Iterate based on feedback**: Refine based on user feedback

### When Performance Is Critical
1. **Measure first**: Profile before optimizing
2. **Consider caching**: Evaluate if caching can improve performance
3. **Optimize database**: Focus on database query optimization
4. **Consider async operations**: Ensure I/O operations are properly async

## Forbidden Actions

### Never Do These Things
- ❌ Break existing functionality without explicit approval
- ❌ Remove or modify existing tests without replacing them
- ❌ Commit secrets, API keys, or sensitive data
- ❌ Hard-code configuration values
- ❌ Skip proper error handling
- ❌ Ignore security best practices
- ❌ Create circular dependencies
- ❌ Violate Clean Architecture principles
- ❌ Remove documentation without replacement
- ❌ Make database schema changes without migrations
- ❌ Use synchronous I/O operations
- ❌ Implement features without tests

### Require Explicit Approval Before
- ⚠️ Breaking changes to existing APIs
- ⚠️ Changes to database schema
- ⚠️ Major architectural refactoring
- ⚠️ Adding new dependencies with security implications
- ⚠️ Changes that affect data migration
- ⚠️ Performance-critical changes that need benchmarking

## Quality Gates

Before considering any task complete, verify:
1. ✅ Solution builds without errors: `dotnet build`
2. ✅ No compilation warnings
3. ✅ Code follows existing patterns and conventions
4. ✅ Tests pass (if tests exist)
5. ✅ No breaking changes to existing functionality
6. ✅ Configuration is properly externalized
7. ✅ Security best practices are followed
8. ✅ Code is documented where necessary
9. ✅ Database migrations are created if schema changed
10. ✅ API documentation is updated if API changed

## Emergency Protocols

### If Something Goes Wrong
1. **Stop immediately**: Halt current work
2. **Assess impact**: Determine what was affected
3. **Communicate**: Inform the user of the issue
4. **Propose solution**: Suggest how to fix the problem
5. **Roll back if needed**: Be prepared to revert changes

### If Unsure About Something
1. **Ask first**: Do not make assumptions
2. **Explain uncertainty**: Clearly state what is unclear
3. **Provide options**: Suggest possible approaches
4. **Wait for guidance**: Do not proceed until clarification is received

## Continuous Improvement

### Learning from the Codebase
- Study existing patterns before making changes
- Understand the "why" behind architectural decisions
- Learn from existing tests to understand expected behavior
- Respect the project's evolution and history

### Adapting to New Requirements
- Maintain architectural integrity while adapting
- Incrementally improve the codebase
- Document why changes were made
- Consider long-term maintainability

---

**This constitution must be followed at all times. When in doubt, err on the side of caution and ask for clarification.**
