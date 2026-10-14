# Expected September 2026 Executive Roll-Up (Answer Key)

## 1. Executive Summary
September portfolio forecasts show two material overruns: P-101 Customer Portal Renewal is forecast at $1,320k against a $1,200k budget (+$120k, +10.0%), and P-102 Data Platform Modernization is forecast at $1,020k against a $900k budget (+$120k, +13.3%). P-101 and P-102 compete for scarce platform engineering capacity while their key milestones are delayed and require decisions by 15–16 October; P-103 remains within budget but its go-live is forecast five days after baseline, and P-104 has a smaller forecast overrun and constrained security test environment. All benefits remain forecasts rather than realized or validated outcomes, and the immediate executive focus is the P-102 contractor funding decision and prioritization of shared engineering capacity.

## 2. Financial & Resource Variance

All amounts are USD thousands. Forecast variance = FAC − approved budget; variance % = variance / budget. Cumulative actual consumed = cumulative actual / budget. Monthly burn = September spend / budget.

| Project | Budget | Sep spend | Cumulative actual | FAC | Variance | Variance % | Actual consumed | Sep burn |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| P-101 Customer Portal Renewal | 1,200 | 130 | 665 | 1,320 | +120 | +10.0% | 55.4% | 10.8% |
| P-102 Data Platform Modernization | 900 | 120 | 535 | 1,020 | +120 | +13.3% | 59.4% | 13.3% |
| P-103 Service Workflow Automation | 650 | 72 | 325 | 630 | -20 | -3.1% | 50.0% | 11.1% |
| P-104 Identity & Access Upgrade | 500 | 52 | 247 | 510 | +10 | +2.0% | 49.4% | 10.4% |
| **Portfolio total** | **3,250** | **374** | **1,772** | **3,480** | **+230** | **+7.1%** | **54.5%** | **11.5%** |

Source: `Finance_September_2026.csv`.

### Three-month trend
- **P-101:** July FAC $1,190k (−$10k, −0.8%); August $1,220k (+$20k, +1.7%); September $1,320k (+$120k, +10.0%). September finance notes cite $120k integration remediation; pilot moved from 15 September baseline to 5 November forecast.
- **P-102:** July $930k (+$30k, +3.3%); August $975k (+$75k, +8.3%); September $1,020k (+$120k, +13.3%). September finance notes cite vendor extension and data-quality remediation.
- **P-103:** July $620k (−$30k, −4.6%); August $640k (−$10k, −1.5%); September $630k (−$20k, −3.1%). It remains forecast under budget, but go-live moved from 30 September baseline to 5 October forecast.
- **P-104:** July $490k (−$10k, −2.0%); August $505k (+$5k, +1.0%); September $510k (+$10k, +2.0%). Security test environment availability remains a delivery risk.

Sources: `Finance_July_2026.csv`, `Finance_August_2026.csv`, `Finance_September_2026.csv`, `Milestones_Benefits_September_2026.csv`.

### Critical resource bottlenecks
- **P-101:** demand 2.0 FTE senior integration engineer, 1.0 available, 1.0 gap through 31 October. Temporary 1.0 FTE allocation for six weeks is requested, not approved.
- **P-102:** demand 2.0 FTE data-quality engineer, 0.5 available, 1.5 gap through 15 November. An 8-week contractor request up to $80k requires COO and Finance Director approval.
- **P-104:** security test environment demand 1.0 versus 0.5 available; two October test windows pending. Current evidence treats this as project-level scheduling unless the date becomes critical.

Sources: `Resource_Constraints_September_2026.csv`, `Risks_Dependencies_September_2026.csv`.

## 3. Strategic Blockers & Escalations
- **P-102 forecast overrun and capacity:** +$120k (+13.3%) forecast variance and 1.5 FTE data-quality capacity gap. DEC-01 requests up to $80k for an 8-week contractor, due 15 October; without approval, Wave 1 remains forecast for 20 October and benefit timing may slip further. Sources: `Finance_September_2026.csv`, `Resource_Constraints_September_2026.csv`, `Decisions_Approvals_September_2026.csv`.
- **P-101/P-102 shared engineering conflict:** scarce capacity is shared; portfolio owner recommends prioritizing P-101 remediation for three weeks, subject to sponsor confirmation. DEC-02 is pending with the COO and due 15 October; no decision leaves both project forecasts exposed. Sources: `Risks_Dependencies_September_2026.csv`, `Decisions_Approvals_September_2026.csv`.
- **P-101 specialist allocation:** +$120k (+10.0%) forecast variance and pilot forecast 5 November versus 15 September baseline. DEC-03 requests 1.0 FTE senior integration engineer for six weeks from the shared pool, due 16 October. This is a resource allocation request, not incremental-spend approval. Sources: `Finance_September_2026.csv`, `Milestones_Benefits_September_2026.csv`, `Decisions_Approvals_September_2026.csv`.

Do not elevate P-103's five-day UAT delay or P-104's test-window booking to portfolio escalation on current evidence.

## 4. Key Decisions & Approvals Needed

| ID | Decision required | Owner | Deadline | Status | Consequence if delayed |
|---|---|---|---|---|---|
| DEC-01 | Approve up to $80k for an 8-week P-102 data-quality contractor, or re-sequence datasets and accept delayed benefits | Maya Chen (COO) and Finance Director | 15 Oct 2026 | Pending | Wave 1 remains forecast for 20 Oct; benefits may slip further |
| DEC-02 | Confirm P-101 priority for shared platform engineering for three weeks, or split capacity across P-101/P-102 | Maya Chen (COO) | 15 Oct 2026 | Pending | Resource contention continues; both forecasts exposed |
| DEC-03 | Allocate 1.0 FTE senior integration engineer to P-101 for six weeks from shared pool | Daniel Brooks, VP Enterprise Platforms | 16 Oct 2026 | Pending | P-101 pilot forecast of 5 Nov remains at risk |

Sources: `Decisions_Approvals_September_2026.csv`, with consequences cross-checked against resource/risk files.
