---
layout: default
title: Create output files
nav_order: 10
parent: Getting Started
---
# Creating output files

Gerber files are sent to a board house for manufacturing.

In Modern PCB Designer, you need to do a single setup once, you select what layers you want to output, and also NC Drill files. 

In the future, every time you want to output the Gerber files you will have to right-click in **Solution** tool window on either the solution node (the root) or the project node and choose **Build** from context menu.

To make this setup, click on **Board properties** then on **Output**, select any layers from the grid that you want to create the Gerber files for. You might have them selected already. Also, click on **Drill files** and make changes, or leave them all default.

Now build the project or solution using **Solution** tool window. After it finishes, in the project folder, you will find a folder called ***!Output***. In this folder you will find all the Gerber files and drill files that you'll send them for manufacture.

## Sending your board for manufacture

It is important to check your Gerber files first, by using a Gerber viewer.

At this moment, we have the gerber and drill files, and we created a zip archive with these.

Next, you will choose a board house.

Most board houses will require to specify for them some details of the board such as: board dimensions, copper finish, soldermask color, and others.

You will have to upload the zip archive mentioned earlier, or, sometimes, send it by mail, you will get a quotation for a price (some have online calculators) and after some time (check this with manufacturer) you will get your PCB at home, ready for it to solder the parts.

