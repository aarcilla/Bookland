# Bookland
## Introduction
Bookland is a database-driven web application inspired by typical online book stores, developed with the ASP.NET MVC framework. It is still work-in-progress.

## Purpose
Solidifying and practice of ASP.NET MVC, database design & development (Entity Framework & SQL Server), and unit testing skills, as well as .NET, C#, and web technology (both server- and client-side) in general.

## Technologies
* **Framework & language**: ASP.NET MVC 4, .NET 4.5 & C# 5.0
* **IDE**: Visual Studio Community 2013, Visual Studio 2012 Express for Web (previous)
* **Source control**: Git, Team Foundation Server 2013 (previous)
* **ORM & DBMS**: Entity Framework 5 Code First with LocalDB SQL Server
* **Unit testing**: Visual Studio Unit Testing with Moq
* **Client-side scripting, validation & DOM interaction**: JavaScript & jQuery

## Features (so far)
Bookland is separated into two sections: the storefront and the admin section.
### Storefront

#### Items
* Browse products available
* Filter products by category (through the tree-based sidebar)
* Order products by name, ID, price, or date added

#### Cart and customers
* Add items to a cart, set item quantities, or remove items
* View basic cart details on any storefront page
* Customer registration and log in for persistent cart (and eventual purchase)

#### User accounts
* Ability to change and reset (through email) your password

### Admin

#### Categories
* Tree-structured category system
* Create category, or directly add a child category for a specific category
* Update or delete categories (which also deletes descendant categories and resets categories for products under that category)

#### Products
* Create, update, or delete products
* Set category for products
* Add image file to associate with a product

#### User accounts
* Four user roles (in order of highest clearance): administrator, support, staff, customer
* Administrators can edit any other users; support can edit staff and customers; staff can only edit customers
* Create and update user accounts

## Developer setup (requires internet access)
1. Clone source code using 'git clone' command, or download as ZIP from GitHub page
2. Open 'Bookland.sln' with at least Visual Studio 2012 Express For Web
3. Download external NuGet packages
	a) Visual Studio > Tools menu > NuGet Package Manager > Package Manager Settings
	b) Under 'Package Restore' heading, ensure the 'Allow NuGet to download missing packages' check box is checked
	c) Visual Studio > Build menu > Build Solution
3. Configure administrator email settings (for sending password reset emails, etc.)
	a) Open the 'Web.config' file in the main Bookland project
	b) Under the 'appSettings' section, edit the value of each email-related setting (denoted as key, e.g. 'smtpHost') based on your nominated email provider and account
	c) Example values (which are made up) are provided for a Gmail-based account
4. Generate database based on project code (i.e. EF Code First)
	a) Visual Studio > Tools menu > NuGet Package Manager > Package Manager Console
	b) In the Package Manager Console, type 'add-migration Init' and wait until completion
	c) Type 'update-database'
	d) You can view the database through Visual Studio's Server Explorer > Data Connections
5. Click on play icon in the main Visual Studio toolbar to interact with Bookland in a web browser
	a) The initial administrator login credentials are:
		User name: admin0
		Password: overlord

## Screenshots
See [screens][0] folder.

 [0]: https://github.com/aarcilla/Bookland/tree/master/screens
