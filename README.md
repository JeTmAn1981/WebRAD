# WebRAD

WebRAD (Web <strong>R</strong>apid <strong>A</strong>pplication <strong>D</strong>evelopment) is a custom forms generator created for the purpose of facilitating form generation for my longtime employer.

<h3>Beginnings</h3>
I originally began creating WebRAD as a response to many requests for simple web forms.  There is a lot of repetition involved in creating a web form.  You need to write markup and validation for each of the form fields.  You need to create database table(s) with columns that match up to your field names, types and lengths.  You need to write stored procedures which reference that same data and know how to insert into or update the database.  You need to write database glue in the backend code to put the field values into the stored procedures to eventually make it into the database.  Then you need to have all these things more or less duplicated for maintenance screens so that form administrators can view and update submitted forms with all the data that accompanies them.

<p>Writing all these components by hand leads to a lot of repetitive and error-prone work.  When I first began the project, there weren't a lot of similar form generators around and even today I don't know of any that could adapt as well to my particular work environment as WebRAD is, given that it's completely customized to be used in that specific environment.  So it was natural enough to begin a project that would remove as much of that duplication as possible.</p>

It didn't take me long before I had support for basic field types (textbox, dropdownlist, radiobuttonlist, etc.)  working, along with database automation and glue code generation.  This was enough to allow me to trim development time on a very simple form from four hours to twenty minutes.  From then on, I knew there was no going back to writing forms by hand (at least not unless they were very complex) and I began to expand the capabilities of WebRAD.  Eventually, it grew to many more features such as:

<ul>
<li>
Multiple pages with the ability to save progress page-by-page.  Login authentication available via Active Directory or custom username/password.
</li>
<li>
Conditionals/actions:  Show/hide other fields based on selection, bind data to control based on selection, use custom code for action, etc.
</li>
<li>
Infinitely nested field templates (e.g. the form allows users to specify any number of guest attendees to an event, each attendee needs to have name and address submitted, guest attendee is a template with all guest info fields specified)
</li>
<li>
A wide variety of field types, many with built-in validation: File upload, image upload, email, type-restricted text input (float only, integer only, etc.), composite fields for adding many related fields at once (contact field adds name, address, email and phone).
</li>
<li>
Many maintenance view features such as sorting, filtering, custom search page, archiving, custom data exports to Excel, scheduling (allowing administrators to setting form to open/close on a schedule), custom reports, and ancillary maintenance projects (create separate projects for maintaining related data such as lists used in the main form, then incorporate the maintenance pages for that ancillary data into the main project).
</li>
<li>
E-commerce support for tying in to the custom e-commerce system I built
</li>
<li>
Email notification to form submitters and/or form supervisors on submission
</li>
</ul>

<h3>Code</h3>

WebRAD is built on and generates projects which use VB.NET and .NET Webforms.  I know, I know, roll your eyes now.  This is the age of React and Vue, serverless and Lambda, isn't it?  That might be true now, but it wasn't when I started this project.  Over time, trends have made some development tools less popular, but that doesn't mean they're less effective.  WebRAD generates full-featured, responsive web forms of almost any complexity and it does it quickly.  If I ever get the time, I would like to modernize the backend generation of the forms themselves to use some newer web development standards(I actually started putting in hooks for generating .NET MVC pages at one point), and preferably use C# as well but as things are WebRAD still achieves very impressive results.  

WebRAD uses a push model, so it creates fully standalone projects that are then pushed out to a live server for consumption.  These projects have zero reliance on WebRAD once they're built.  You can modify them in any way desired.  This also makes WebRAD good for generating scaffolding for a project that you plan to take further with custom development.

As for the quality of the code itself, that is not high.  As I've mentioned, I began this project a long time ago and I've come a long way as a developer since then.  So much of the codebase itself is quite ugly and full of things that make me wince, things I would rip out and refactor if I had the time.  There are no tests to speak of.  But nearly as much of my time with WebRAD is spent generating projects for end-users as it is developing WebRAD itself.  I have refactored some of the most essential page-generating classes so those are a better example of good software development practice. These can be found in the Common/Libraries/Webpages folder.

Despite the poor quality of much of the code, this by far the most effective software project I've ever been involved in from the point of view of saving time and allowing me to generate a wide variety of work with comparatively little wasted development time.  As a tool, WebRAD has been extremely effective.

