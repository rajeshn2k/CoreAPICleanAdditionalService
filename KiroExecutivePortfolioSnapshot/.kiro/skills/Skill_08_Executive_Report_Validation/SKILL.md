---
name: executive-report-validation
description: "Validates report structure, summary sentence count, financial arithmetic, source traceability, portfolio coverage, and executive relevance."
---

# Skill 8 — Executive Report Validation

## Purpose
Validate the Executive Portfolio Snapshot for structure, arithmetic, traceability, coverage, and executive relevance before human review.

## Procedure
1. Confirm exactly four sections in this order:
   1. Executive Summary
   2. Financial & Resource Variance
   3. Strategic Blockers & Escalations
   4. Key Decisions & Approvals Needed
2. Confirm the Executive Summary has exactly three sentences.
3. Check material claims against authorized sources and data dates.
4. Recalculate financial figures from recorded inputs where possible.
5. Check currency, scope, periods, denominators, signs, units, and rounding.
6. Compare project coverage with the authoritative portfolio register.
7. Confirm benefits are labelled as targets, forecasts, or realized results accurately.
8. Check that resource bottlenecks and escalations are material and evidence-based.
9. Confirm pending decisions are not described as approved and deadlines/approvers are not invented.
10. Flag unsupported claims, stale data, conflicts, and unknowns.
11. Remove tactical details that do not require executive attention.
12. Confirm title: "Draft Monthly Executive Portfolio Snapshot — For Review".

## Required output
Validation result (Pass, Pass with warnings, or Needs correction), structure check, summary sentence count, financial checks, traceability gaps, coverage gaps, unsupported/conflicting claims, and corrections needed.

If the user requests only the final report, do not add a separate validation section to the report. Apply checks before returning it and preserve the exact four-section format.

## Guardrails
Do not fabricate evidence, hide conflicts, change source records, or publish the report. A formatting pass does not prove the underlying data is correct.

## Completion check
Structure is correct, calculable figures are checked, material claims are traceable, and unresolved limitations are visible.
