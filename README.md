Integrating Windows Azure Web Sites and On-Premise Applications
=================
Throughout this session, the example of Enterprise Pizza, a fictitious pizza company, is building their pizza-ordering site atop Web Sites. They have a series of requirements that result in the reality that their site will need to tie into their store in some way or another and that their goals can't be met if they can't have direct communication between individual stores. 

## Storyline ##

The demos build the site's story. 

![](scenario.png)

Some of the details of the storyline are listed below. 

1. Site, domain project, and data-access project for persisting data to the database
1. Orders sent to corporate office application via a Service Bus Topic
1. Orders saved to corporate database using EF/DAL functionality
1. Orders sent to store using Service Bus Topics so the orders can be displayed on a monitor application
1. Inventory service hosted in web site and in corporate app that is called by the internal monitor application when store workers want to update the inventory
1. Order status service hosted in web site that provides real-time order status to the customer as the store creates and ships the pizza 