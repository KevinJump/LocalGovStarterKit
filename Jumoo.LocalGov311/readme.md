Localgov Open311 - Inquiry API for Umbraco
==

An initial draft of the [Open311 Inquiry](wiki.open311.org/Inquiry_v1) protocol for umbraco sites. 

The plugin produces a Open311 compatible xml (or json) file listing all the 
services within a site, because this has a slight UK bias, it also allows you
to use the [ESD Service ID](http://standards.esd.org.uk/) as the ID for your service, this means service ids 
would be consistant between localgov sites, so you could collate information 
betterer.

by default a call to /Open311/Inquiry/ will list all services with an ESD ID defined in a field, you will get something like...
```
<services xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <service>
    <id>123</id>
    <service_name>Benefits and Grants</service_name>
    <brief_description>
      Including Benefits advice, Housing Benefits, Council tax support, education grants.
    </brief_description>
    <modified>2015-10-27T10:58:40</modified>
    <expiration>2025-10-28T11:28:03.1161459+00:00</expiration>
    <url>http://localhost:55898/benefits-and-grants/</url>
  </service>
  <service>
    <id>43</id>
    <service_name>Council Tax</service_name>
    <brief_description>
      Includes pay council tax, council tax bands and single person discounts.
    </brief_description>
    <modified>2015-10-27T11:42:49</modified>
    <expiration>2025-10-28T11:28:03.2482632+00:00</expiration>
    <url>http://localhost:55898/council-tax/</url>
  </service>
</services>
```

you can also request a single serviice /Open311/Inquiry/123 and you get a slightly more detailed response (not all the open311 fields yet)

```
<services xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <service>
    <id>123</id>
    <service_name>Benefits and Grants</service_name>
    <brief_description>
      Including Benefits advice, Housing Benefits, Council tax support, education grants.
    </brief_description>
    <description>
      <p>A more detailed description of the benefits and grants page.</p>
    </description>
    <modified>2015-10-28T12:38:15</modified>
    <expiration>2025-10-28T12:38:30.0613358+00:00</expiration>
    <url>http://localhost:55898/benefits-and-grants/</url>
  </service>
</services>
```

Installing
==
this package isn't part of the wider localgov starterkit just yet, you will need to install it seperately. 

NuGet
--

> Install-Package Open311.Umbraco -Pre

Manual
--

* copy the Jumoo.LocalGov311.dll to the bin folder
* copy the contents of App_Plugins into App_Plugins on your site
* add a dashboard section (below) to the end of dashboard.config to see the dashboard

```
  <section alias="Open311Settings">
    <areas>
      <area>settings</area>
    </areas>
    <tab caption="Open311">
      <control>
        ~/App_Plugins/Open311/Open311Settings.html
      </control>
    </tab>
  </section>
```
