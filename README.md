# Bookland
## Introduction
Bookland is a database-driven web application inspired by typical online book stores, developed with the ASP.NET MVC framework. It is still work-in-progress.

## Technologies
* **Framework & language**: ASP.NET MVC 4 & C# 5.0
* **IDE**: Visual Studio 2012 Express for Web
* **Source control**: Team Foundation Server 2013
* **ORM & DBMS**: Entity Framework Code First with LocalDB SQL Server
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
* Three user roles (in order of highest clearance): administrator, staff, customer
* Administrators can edit any other users; staff can only edit customers
* Create and update user accounts

## Screenshots
See [screens][0] folder.

 [0]: https://github.com/aarcilla/Bookland/tree/master/screens
