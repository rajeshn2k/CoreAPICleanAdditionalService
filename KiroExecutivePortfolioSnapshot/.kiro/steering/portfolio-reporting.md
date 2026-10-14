---
inclusion: always
---

# Executive Portfolio Snapshot — Reporting Rules

## Purpose
Generate a concise, evidence-based monthly executive portfolio report using the available portfolio context and monthly source evidence.

## Required report structure
Produce exactly these four top-level sections in this order:

1. Executive Summary — exactly three sentences.
2. Financial & Resource Variance.
3. Strategic Blockers & Escalations.
4. Key Decisions & Approvals Needed.

## Evidence rules
- Use Portfolio Charter, Project Register, Metric Definitions, and Portfolio Governance and Authority as context sources.
- Use monthly CSV evidence for actual spend, forecasts, milestones, benefits, risks, resource constraints, and decisions.
- Cite source filenames and reporting periods for material claims.
- Distinguish actual spend, forecasts, forecast benefits, and realized benefits.
- Do not treat pending decisions or proposed funding as approved.
- Identify missing, conflicting, stale, or unsupported evidence explicitly.
- Never invent missing figures, owners, deadlines, approvals, or outcomes.

### Evidence catalogue
- `02_Source_Evidence/Source_Register.md`

Purpose:
- Use this register to identify the expected evidence files, reporting periods, source ownership where documented, and evidence limitations.
- Verify that expected files exist and are accessible before generating a report.
- Use the detailed CSV registers as the primary source for record-level financial, milestone, resource, risk, and decision values.
- Use the Source Register to discover files; do not treat its descriptions as substitutes for reading the underlying evidence.
- If the register lists a missing file, disclose the gap and its effect on the report.
- If additional relevant evidence files exist that are not listed, inspect them when appropriate and flag the discrepancy.

## Financial rules
- Forecast variance = forecast at completion minus approved budget.
- Forecast variance percentage = forecast variance divided by approved budget, multiplied by 100.
- Cumulative actual budget consumed = cumulative actual spend divided by approved budget, multiplied by 100.
- Monthly burn rate = reporting-month spend divided by approved budget, multiplied by 100.
- Use Metric Definitions as the authoritative source if it specifies different formulas or rounding rules.
- Validate calculations and show currency units clearly.

## Reporting behavior
- Include all projects in the Project Register.
- Compare with previous months when evidence is available.
- Prioritize material portfolio risks, resource conflicts, decisions, owners, deadlines, and consequences of delay.
- If required evidence cannot be accessed, state what is missing and do not pretend the report is complete.

## Source File Manifest

All paths are relative to the ExecutivePortfolioSnapshot workspace root.

### Permanent portfolio context
- `01_Portfolio_Context/Portfolio_Charter.md`
- `01_Portfolio_Context/Project_Register.md`
- `01_Portfolio_Context/Metric_Definitions.md`
- `02_Source_Evidence/Portfolio_Governance_and_Authority.md`

### Monthly evidence
Reporting month: September 2026
Comparison months: July 2026 and August 2026

Read the relevant files from:
- `02_Source_Evidence/Monthly/`

Inspect the filenames and identify the files corresponding to the reporting month and comparison months. Include all relevant evidence categories, such as finance, milestones and benefits, resources, risks, decisions, and portfolio notes, where present.

02_Source_Evidence/Portfolio_Notes_September_2026.md

Role: Portfolio Management Office summary and evidence-gap register.

Use this file to identify cross-project themes, escalation priorities, and known data limitations. Verify material financial figures, milestone dates, resource gaps, decision owners, deadlines, and approval statuses against their authoritative source files. If this summary conflicts with a source register, identify the conflict rather than silently choosing a value.

### Validation-only files
- `05_Expected_Reports/Expected_September_2026_Executive_Rollup.md`
- `05_Expected_Reports/Answer_Key_Calculations.md`

These files are for validation after the report has been generated. Do not use them as source evidence, copy their narrative, or use their expected values to replace calculations from the original evidence.

## Source selection and duplicate prevention

1. Treat the four permanent context files as shared context; do not reproduce their full contents in the report.
2. Read each physical source file only as needed. Do not add the same source file twice through overlapping file and folder references.
3. If a file is referenced by more than one instruction, treat it as one source with one identity.
4. Extract the required facts from the source files and cite filenames and reporting periods in the report.
5. Do not infer that a file exists or has been read merely because it appears in this manifest.
6. Before reporting, list any required files that are missing, inaccessible, malformed, or ambiguous.
7. Do not treat validation-only files as evidence.

Portfolio notes summarize information from other evidence sources. Do not count a risk, resource gap, milestone, decision, cost, or benefit twice merely because it appears in both portfolio notes and a detailed source file. Use detailed registers for record-level values and use portfolio notes for cross-project context and documented evidence gaps. Cite both sources when the summary adds material context not present in the detailed register.

## Report Output and File Management

### Output location
- Save generated reports under `06_Generated_Reports/`, relative to the workspace root.
- Use the filename pattern `Executive_Portfolio_Snapshot_<Month>_<Year>.md`.
- Example: `06_Generated_Reports/Executive_Portfolio_Snapshot_September_2026.md`.

### File creation rules
1. Generate the report using the required four-section structure and all applicable evidence and calculation rules.
2. Write the complete report to the designated Markdown file, including tables, source filenames, reporting periods, and relevant evidence limitations.
3. Create the output directory if it does not exist and the workspace permissions allow it.
4. Do not modify source evidence, steering instructions, skills, prompts, or expected reports while generating the output.
5. If the destination report already exists, replace only that month's generated report when the user has explicitly requested regeneration. Otherwise, ask before overwriting it.
6. Do not save an incomplete report as final without clearly identifying missing evidence and limitations. If a required source is unavailable, save a clearly labelled draft only when appropriate and disclose its status.
7. After writing the file, verify that it exists and that its contents contain all four required report sections.
8. If file creation or verification fails, explain the failure and do not claim the report was saved.

### Chat response after generation
- Provide the exact relative path of the saved file.
- State whether the file was successfully created and verified.
- Summarize any important missing evidence or validation failures.
- Do not repeat the entire report in chat unless the user requests it.