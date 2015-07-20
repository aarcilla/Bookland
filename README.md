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
* Basic product search

#### Cart and customers
* Add items to a cart, set item quantities, or remove items
* View basic cart details on any storefront page
* Customer registration and log in for persistent cart (and eventual purchase)
* Checkout process, including email of HTML invoice

#### User accounts
* Ability to change and reset (through email) your password
* Ability to view past purchases

### Admin

#### Categories
* Tree-structured category system
* Create category, or directly add a child category for a specific category
* Update or delete categories (which also deletes descendant categories and resets categories for products under that category)

#### Products
* Create and update products
* Set category for products
* Add image file to associate with a product

#### User accounts
* Four user roles (in order of highest clearance): administrator, support, staff, customer
* Administrators can edit any other users; support can edit staff and customers; staff can only edit customers
* Create and update user accounts
* View their past purchases

## Planned features
* Author table and associated Product field: view products by author, different author types (e.g. artist, brand)
* Format table and associated Product field (e.g. DVD, Blu-ray, Paperback, XBox One)
* Stock management and processes (e.g. checkout reduces stock count of bought product)
* Sale (price reduction) management for products: sale time interval, generate sales by category, discount type (e.g. x% off, $x off)
* Categorised, configurable front page (e.g. featured products, on sale, recently added)
* "Infinite scroll" pagination (utilising JS and AJAX) for Home page and admin Product page
* Edit multiple Products at once with same field values (e.g. Status, Category)

## Developer setup
N.B. Requires internet access

### 1. Retrieve source code
1. Clone source code using 'git clone' command, or download as ZIP from GitHub page

### 2. Open project
1. Open 'Bookland.sln' with at least Visual Studio 2012 Express For Web

### 3. Download external NuGet packages
1. Visual Studio > Tools menu > NuGet Package Manager > Package Manager Settings
2. Under 'Package Restore' heading, ensure the 'Allow NuGet to download missing packages' check box is checked
3. If you are running a Visual Studio edition earlier than 2013: Right-click "Solution 'Bookland'" in the Solution Explorer (usually on the right-hand side in Visual Studio) > Enable NuGet Package Restore > Yes button
4. Visual Studio > Build menu > Build Solution
5. Close Visual Studio

### 3. Configure administrator email settings (for sending password reset emails, etc.)
1. Re-open 'Bookland.sln' with Visual Studio
2. Within the Solution Explorer, open the 'Web.config' file in the main Bookland project
3. Under the 'appSettings' section, edit the value of each email-related setting (denoted as key, e.g. 'smtpHost') based on your nominated email provider and account
4. If you would rather not use or bother to configure email, set the 'emailEnabled' setting to 'false'; keep in mind email-reliant features, including password reset and checkout invoice, won't work
5. Example values (which are made up) are provided for a Gmail-based account

### 4. Generate database based on project code (i.e. EF Code First)
1. Visual Studio > Tools menu > NuGet Package Manager > Package Manager Console
2. In the Package Manager Console, type 'add-migration Init' and wait until completion
3. In the Package Manager Console, type 'update-database'
4. You can view the database through Visual Studio's Server Explorer > Data Connections

### 5. Click on play icon in the main Visual Studio toolbar to interact with Bookland in a web browser
The initial administrator login credentials are:

* User name: admin0
* Password: overlord

## Screenshots
Available for view and download at [this][0] gallery.

 [0]: https://www.dropbox.com/sh/dj3iqw0dpl13h5h/AAArf0o7dFAFYSMeBzXHebvza?dl=0
