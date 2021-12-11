# MultiFamilyPortal

The MultiFamilyPortal is configured as a Multi-Tenant SaaS application. This means that there is a single deployed instance of the application, with a unique database connection for each tenant. To facilitate this, the MultiFamilyPortal relies on two Database Connections:

- The Default Connection: This is a templated connection string in which the Initial Catalog or Database name must be provided as `{0}`. The name of the database configured for the tenant will be substituted for each tenant.
- The Tenant Connection: This is database that controls allowed tenants.

Both connection strings must be supplied for the MultiFamilyPortal to function. However only the Tenant Connection is required for the MultiFamilyPortal SaaS Admin portal. By default while running this locally in debug mode, the MultiFamilyPortal will ensure that a Tenant exists for the host `localhost`. This will default to a database named `multifamilyportal`. You can customize this using the Admin portal.

You must apply the database migration for the Tenant Portal. The MultiFamilyPortal will automatically apply all unapplied Migrations for all tenants. During the startup the Startup tasks will run ensure that default resources have been created for the tenants, and that the database is seeded with all expected defaults.

## Configuration

The MultiFamilyPortal expects a configuration as follows.

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "{Google Client Id}",
      "ClientSecret": "{Google Client Secret}"
    },
    "Microsoft": {
      "ClientId": "{Microsoft Client Id}",
      "ClientSecret": "{Microsoft Client Secret}"
    }
  },
  "Captcha": {
    "SiteKey": "{Captcha Site Key}",
    "SecretKey": "{Captcha Secret Key}"
  },
  "SendGridKey": "{SendGrid Key}",
  "ConnectionString": {
    "DefaultConnection": "{Default Connection String}",
    "TenantConnection": "{Tenant Connection String}"
  }
}
```

The SaaS Admin portal only requires the Tenant Connection String.

### Testing Multi Tenant Functionality

In order to locally test the Multi Tenant Functionality, you must provide an updated `hosts` configuration on your local machine. For instance let's say that you want to test the following two demo tenants:

- demoportal
- acmeequities

To do this you would provide an updated `hosts` configuration as follows:

```
127.0.0.1   demoportal
127.0.0.1   acmeequities
```

#### On Mac

You can edit the `hosts` file using the following command:

```bash
sudo nano /private/etc/hosts
```

#### On PC

Open the file `C:\Windows\System32\drivers\etc\hosts` and edit the file. You may do this using Notepad or Visual Studio Code. Note that if you are using Notepad you will need to run Notepad as the Administrator. If you are using Visual Studio Code, when saving or "Auto Saving" you may be prompted to "Retry as Administrator". It will require elevated privileges to save the file.

## Limitations

Currently the SaaS functionality is limited to only preparing the database for tenants at application startup and cannot prepare tenant databases from the SaaS Admin Portal or on Client Requests. This is intentional as the preparation process is expensive and should not be performed in the request pipeline. Additional work will likely need to be done to migrate this functionality to the SaaS Admin Portal.
