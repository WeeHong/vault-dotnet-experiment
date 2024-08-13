Certainly! Here's the updated README with the new features included:

---

# Vault .NET Experiment

The Vault .NET Experiment is designed to demonstrate how to integrate HashiCorp Vault with .NET applications, including capabilities for token management and secure secret retrieval. This project provides a practical example of managing secrets securely, automatically renewing tokens, and using AppRole for authentication.

## Overview

This experiment showcases the integration of HashiCorp Vault with .NET applications. Key features include secure secret retrieval, automatic token renewal, creating and using AppRole tokens, and fetching key-value (K/V) secrets from Vault.

## Features

- **Integration with HashiCorp Vault**: Access and manage secrets from within .NET applications.
- **Token Renewal**: Automatically renew Vault tokens at specified intervals to ensure uninterrupted access.
- **Create AppRole Token**: Generate and manage AppRole tokens for secure application authentication.
- **Fetching K/V Secret**: Retrieve key-value secrets stored in Vault using .NET, with support for AppRole authentication.
- **Sample Code**: Includes examples and best practices for integrating Vault with .NET.

## Prerequisites

Before you start, ensure you have the following:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [HashiCorp Vault](https://www.vaultproject.io/downloads) (version 1.8.0 or later)
- Basic knowledge of .NET and HashiCorp Vault

## Getting Started

1. **Clone the Repository**

   ```bash
   git clone https://github.com/your-username/vault-dotnet-experiment.git
   cd vault-dotnet-experiment
   ```

2. **Set Up Vault**

   - Install and start Vault on your machine or use a remote Vault server.
   - Configure your Vault server with the necessary secrets, policies, and AppRole setup.

3. **Configure the Project**

   - Open the `appsettings.json` file and enter your Vault server address, token, and other required settings.
   ```
    {
      "Vault": {
        "Address": "http://localhost:8200",
        "RoleId": "role-id"
      }
    }
   ```
   - Update the `_interval` variable on the `VaultTimerService.cs` and other settings on `VaultClient.cs` to define how often the token should be renewed and configure AppRole authentication details.

4. **Build and Run**

   ```bash
   dotnet build
   dotnet run
   ```

5. **Review the Code**

   - Check `Program.cs` for examples on how to interact with Vault, renew tokens, create AppRole tokens, and fetch K/V secrets.
   - Modify the sample code as needed to fit your specific use case.

## Usage

- **Token Management**: The project demonstrates how to programmatically renew Vault tokens to ensure that they remain valid and avoid expiration issues.
- **AppRole Authentication**: Learn how to create and use AppRole tokens for secure application authentication with Vault.
- **Secret Access**: Retrieve secrets from Vault using AppRole tokens and .NET, allowing for secure access to sensitive information.
- **Fetching K/V Secret**: Example code is provided to show how to fetch key-value pairs from Vault, enabling secure retrieval of configuration data and sensitive information.

## Additional Information
For a detailed article on this project, including in-depth explanations and use cases, visit [Vernon Wee Hong's Blog](https://vernonweehong.com/blog).

## Contributing

Contributions are welcome! Feel free to submit issues, pull requests, or suggestions to improve the project.

## License

This project is licensed under the MIT License.

## Contact

For questions or feedback, please open an issue on GitHub or contact [vernonweehongkoh.developer@outlook.com](mailto:vernonweehongkoh.developer@outlook.com).

---

Feel free to adjust the repository URL, contact email, and any other project-specific details as necessary.