# FluidTemplating
A proof-of-concept (POC) .NET 8 console app that demonstrates how to build and render dynamic reports using `Fluid templates`, `Microsoft Playwright` for PDF generation, and `UglyToad.PdfPig` for extracting page numbers to build a Table of Contents.

## Frameworks and Libraries

- __[Fluid](https://github.com/sebastienros/fluid?utm_source=chatgpt.com)__

   Used as the templating engine. Fluid provides Liquid-compatible syntax `({{ variable }}, {% for %})` to mix static content with dynamic data (`FakeDbTables\...`).

- __[Playwright for .NET](https://playwright.dev/dotnet/)__

  Used to render HTML + CSS into PDF output. We chose Playwright because it supports modern CSS, custom fonts, headers/footers, and is reliable across platforms.

- __[UglyToad.PdfPig](https://github.com/UglyToad/PdfPig)__

  Used to extract page numbers after an initial render pass. This allows generating a Table of Contents (ToC) with actual page references.Playwright itself cannot easily extract page numbers after rendering, so PdfPig fills this gap.

## Flow of the POC

1. Start the app

   The console app will prompt what you want to do:
   - Add/update variables (saved in `Data/variables.json`)
   - Add/update sections (saved in `Data/sections.json`)
   - Create templates (saved in `Data/templates.json`)
   - Bind templates to assessments
   - Render a report

2. Variables

   Variables are key-value pairs (e.g. company: "My Business") stored in JSON. They can be referenced in Fluid templates like:

   ```
   
   Introdution for {{ company }}

   ```

3. Sections

   Sections are built with specific building blocks (Text, Table, Image, List). 
   - Each section is stored in JSON (`sections.json`).
   - Sections can contain Fluid markup for dynamic data (e.g. {{ report.Assessment.ClientName }}).
   - Blocks can be static or dynamic (tables/lists generated from dynamic data).

4. Templates

   Templates reference one or more sections in a chosen order.
   - A template JSON maps sections into a “report structure.” No visual elements are aplied.
   - At render time, the template + variables + dynamic data are combined.

5. Rendering

   Rendering is a two-phase process:
   - Phase 1: Build HTML, render to temporary PDF, extract headings/pages.
   - Phase 2: Inject a Table of Contents with correct page numbers and regenerate the final PDF.

Output: `report.pdf` in bib/.net8. (reality user will choose output path)

## Visual Themes (CSS)

Visual styling is applied via `CSS` files located in the `Templates/` folder (e.g. report.css, orange.css). At render time you can choose which `CSS` file to apply.

To add a new visual theme:
- Create a new `CSS` file in `Templates/` (e.g. modern.css).
- Override fonts, colors, spacing, etc.
- Select it when running the app (cssFileName parameter).

## JSON Structure

Variables (variables.json)

```json

{
  "company": "Acme Corp",
  "contact": "security@acme.com"
}

```

Sections (sections.json)

```json

[
  {
    "Id": "guid-here",
    "Name": "Introduction",
    "Order": 1,
    "Children": [
      {
        "Type": 1,
        "Data": {
          "Level": 1,
          "HeadingTemplate": "Findings for {{ report.Assessment.ClientName }}",
          "BodyTemplate": "{{ report.Assessment.Summary }}"
        },
        "Order": 100
      }
    ]
  }
]

```

Templates (templates.json)

```json

[
  {
    "Id": "guid-here",
    "Name": "StandardReport",
    "SectionReferences": [
      { "SectionId": "guid-section1", "Order": 1 },
      { "SectionId": "guid-section2", "Order": 2 }
    ]
  }
]

```

## Example Output

- Cover page with logo and client details
- Disclaimer page with address and legal notice
- Table of Contents with clickable links + page numbers
- Dynamic report sections (findings, tables, lists, images)
- Consistent styling based on chosen CSS theme