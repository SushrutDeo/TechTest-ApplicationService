# Application Service

## Background

A fintech company distributes `ProductOne` using an external company, `AdministratorOne`, to manage the Investors, Accounts and Payments.

The company wants to launch `ProductTwo` using `AdministratorTwo`.  

In the future, the company might use `AdministratorTwo` for `ProductOne`.  And, of course, you can imagine the company will want to quickly launch more Products.

### Goal

The goal of this exercise is to process Applications for both Products, ensuring the appropriate Administrator is used to create an Investor and Account, and to process the initial Payment.

### Requirements
- Implement `IApplicationProcessor`
- Only valid Applications should be processed
    - `ProductOne` is only available to people aged 18 to 39.
    - `ProductTwo` is only available to people aged 18 to 50.
    - The minimum Payment for each Product is currently £0.99 but likely to change.
- New Users must be Kyc'd.
- Only Applications from verfied Users should be processed.
- All outputs of the Application process must be captured by downstream systems which subscribe to `DomainEvents`. Some examples are included.

### Notes

- `Services.AdministratorOne.Abstractions` is a third-party library.
- You may add new projects to the solution, and add appropriate nuget references.
- You may change and add new types defined in `Services.Common.Model`. But, because this library is shared within the company, you'll need to justify your decision.
- The exercise should take no-longer than two hours.  Don't implement:
    - Simple validation (required, min-length etc).
    - Argument checking
    - Logging
- Keep a simple log of your decisions etc.  See [log](log.md)
- Commit often to demonstrate your thought process.  
- When you're ready, push and let Tembo know when you're done.

