# Executive Roll-Up Agent — Fictional Portfolio Exercise

**Reporting month:** September 2026 | **Trend window:** July–September 2026 | **Currency:** USD thousands  
All names, project data, figures, dates, and events are fictional.

## Files
- `01_Portfolio_Context/` — portfolio charter, project register, metric definitions.
- `02_Source_Evidence/Monthly/` — monthly finance and milestone/benefit snapshots for July, August, September; September resource, risk, dependency, and decision registers.
- `02_Source_Evidence/` — source register and authority rules.
- `03_Agent_Instructions/` — agent instructions and validation skill.
- `04_Prompt_Library/` — initial report prompt, follow-up queries, and governance tests.
- `05_Expected_Reports/` — expected September report and calculation answer key.

## Execute the exercise
1. Create a Microsoft 365 Copilot agent named **Executive Roll-Up**.
2. Copy `03_Agent_Instructions/Executive_Rollup_Agent_Instructions.md` into the agent's instruction field, adjusting for your tenant's instruction limits.
3. Add `01_Portfolio_Context/` and `02_Source_Evidence/` as knowledge. Recommended: upload them to a dedicated SharePoint document library and connect that location if supported by your tenant.
4. Keep `04_Prompt_Library/` as your test prompts. It is not authoritative evidence.
5. Ask the agent the prompt in `04_Prompt_Library/01_Generate_September_Report.md`.
6. Compare the output to `05_Expected_Reports/Expected_September_2026_Executive_Rollup.md`.
7. Run each prompt in `04_Prompt_Library/02_Follow_Up_Queries.md` individually.
8. Test evidence discipline with `04_Prompt_Library/03_Governance_and_Quality_Tests.md`.

## Identify the report
The expected target is `05_Expected_Reports/Expected_September_2026_Executive_Rollup.md`. Output must have exactly four sections in this order: Executive Summary (exactly 3 sentences), Financial & Resource Variance, Strategic Blockers & Escalations, Key Decisions & Approvals Needed.

A correct report cites source filenames and periods, separates actual spend from forecasts, calculates metrics correctly, covers all four projects, and does not treat pending approvals or forecast benefits as facts already realized.

**Note:** Uploading Markdown as knowledge does not automatically register custom Copilot skills. Use the agent instruction field for the main instructions; register separate skills only if your Copilot environment explicitly supports them.

## 4. Save and manage generated reports

Generated reports are saved under `06_Generated_Reports/`.

For September 2026, the expected output path is:

`06_Generated_Reports/Executive_Portfolio_Snapshot_September_2026.md`

The reporting prompt instructs Kiro to:
1. Read the applicable source evidence and follow the workspace steering instructions.
2. Generate the report using the required four-section structure.
3. Save the complete report to the designated Markdown file.
4. Verify the file contents and required section structure.
5. Confirm the saved path in Kiro Chat and disclose any outstanding issues.

Do not overwrite an existing report without authorization. To regenerate a report, explicitly ask Kiro to replace the generated report for that reporting month.

Keep generated reports separate from `05_Expected_Reports/`. The expected report and answer key are used for validation only; they must not be treated as reporting evidence.

For future reporting periods, create or update the corresponding prompt and use the appropriate month and year in the output filename.