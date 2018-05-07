# The Business Reviewer

![logo](art/ic_bitcommunity.png)

This is a Xamarin app that gives users the opportunity login and create reviews for businesses and or restaurants. In addition to being able to leave a text review, the user can also upload photos and videos.

The app showcases how to use several Azure offerings in the context of a mobile application - including how to create accounts and stream videos.

## Features

This project demonstrates the following:

* Xamarin.Forms
* MVVM architecture
* Azure AD B2C
* Azure App Services
* Azure Cosmos DB
* Azure Functions
* Azure Storage
* Azure Media Services

## Getting Started

To get started, clone this repository and then follow the directions in the Quick Start section below to setup the Azure infrastructure. Once done, you will be able to run the app.

### Prerequisites

* Visual Studio or Visual Studio for Mac.
* Azure subscription (create a free one here.)

### Installation

* Clone this repository.
* Open the `/src/Reviewer.sln` in either Visual Studio or Visual Studio for Mac.
* Restore all NuGet packages.

### Quickstart

In order to run this full sample, an Azure subscription is required. You can create a free subscription here.

> Note: When creating the Azure offerings below - use the same Resource Group. And when given the option to pick a Storage Account - pick the same one for each.

Once you have a subscription, you will need to create several Azure offerings:

#### Azure Storage

1. Create an Azure Storage Account.

#### Azure AD B2C

Setting up Azure AD B2C is the most involved portion.

1. First you need to create the tenant.
1. Next you need to create your application.
    1. Within the application, create a scope, and name it `rvw_all`.
1. Next you need to add any identity providers.
1. Then you need to create a sign-up/sign-in policy.

#### Azure Cosmos DB

1. Create an Azure Cosmos DB - SQL API - instance.
1. Create a database named: `BuildReviewer`
1. Create a collection named: `Businesses`
1. Create a collection named: `Reviews`

#### Azure App Service

1. Create the Azure App Service.
1. Configure the `Reviewer.WebAPI` project's `appsettings.json` file to match the following:

    ```language-javascript
    "AzureAdB2C":
    {
        "Instance": "https://login.microsoftonline.com/tfp/",
        "ClientId": "",
        "Domain": "",
        "SignUpSignInPolicyId": "",
        "AllAccessScope": "rvw_all"
    }
    ```

    * The `ClientId` value will be the __Application ID__ of your Azure AD B2C application.
    * The `Domain` will be the __Domain Name__ of your Azure AD B2C tenant.
    * The `SignUpSignInPolicyId` will be the name of the sign-up/sign-in policy you created.

1. Update the following variable values in `Reviewer.Services.APIKeys`:
    1. `CosmosEndpointUrl`: Obtained from the API Keys blade of the portal for Cosmos DB.
    1. `CosmosAuthKey`: The _primary key_ from the API Keys blade of the portal for Cosmos DB.
    1. `WebAPIUrl`: The URL of this Azure app service, can be obtained from the overview blade in the portal. (Make sure to include the trailing backslash.)
1. Deploy the `Reviewer.WebAPI` ASP.NET Core Web API application to the Azure App Service instance.

#### Azure Media Services (AMS)

1. Create an Azure Media Services instance.
1. Use the same Azure Storage account as created above.
1. [Start the default streaming endpoint](https://msou.co/bh3).
1. Create an [Azure AD application](https://msou.co/bh5) for it.
1. Create an Azure AD [service principal](https://msou.co/bh4).

#### Azure Functions

1. Create an Azure Function App.
1. Use the same Azure Storage account as created above.
1. Create the following __Application Settings__ keys with values:
    1. `AMSAADTenantDomain`: The Azure AD domain you created for the AMS app in step 4 in the Azure Media Services setup above.
    1. `AMSClientId`: The Azure AD client ID for the Azure Media Services application you created during the Azure Media Services setup above.
    1. `AMSClientSecret`: The secret key obtained during the Azure Media Services AD creation steps above.
    1. `AMSRESTAPIEndpoint`: Obtained on the Azure Media Services overview blade in the portal.
    1. `MediaServicesStorageAccountKey`: Obtained on the __Primary Storage ID__ of the AMS Properties blade.
    1. `MediaServicesStorageAccountName`: Obtained on the __Primary Storage Name__ of the AMS Properties blade.
    1. `Reviews_Cosmos`: The Azure Cosmos DB connection string (obtained on the keys blade for Cosmos DB).
    1. `WebhookEndpoint`: This will need to be obtained after you deploy the Function app. It is the URL of the __AMSWebhook__ function.

> Note that all of those settings can also be put into your local.settings.json to debug locally.

#### Xamarin.Forms Project

Finally there are a couple of settings that you need to configure in the Xamarin.Forms project to ensure it can communicate to the Azure offerings.

1. In the `Reviewer.Services.APIKeys` class fill in the values for the following variables:
    1. `SASRetrievalUrl`: The function URL of the __SASRetrievalURL__ function.
    1. `WriteToQueueUrl`: The function URL of the __WritePhotoInfoToQueue__ function.
    1. `StorageAccountName`: Within the Azure Storage service - the __Storage Account Name__ as found on Access Keys blade.
    1. `PhotosContainerName`: __review-photos__

## Demo

There are a couple things to note when running the demo.

1. The Xamarin.Forms app needs to run with an Android version that supports custom tabs.

## Resources

There will be links here eventually to relevant documentation for further reading.