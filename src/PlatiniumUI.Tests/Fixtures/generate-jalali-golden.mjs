// Golden-fixture generator for the OK Pars Jalali engine.
// Uses the SAME Intl('en-u-ca-persian', Asia/Tehran) implementation as
// server/utils/jalali.ts, so the .NET port is validated against real ground truth.
// Run: node generate-jalali-golden.mjs > jalali-golden.json
const faFmt = new Intl.DateTimeFormat('en-u-ca-persian', {
  timeZone: 'Asia/Tehran', year: 'numeric', month: 'numeric', day: 'numeric',
})
function jalaliParts(d) {
  const p = {}
  faFmt.formatToParts(d).forEach(x => { if (x.type !== 'literal') p[x.type] = x.value })
  return { year: Number(p.year), month: Number(p.month), day: Number(p.day) }
}
const DAY = 86400000
function jalaliMonthStart(jy, jm) {
  let g = new Date(Date.UTC(jy + 621, 2, 20) + (jm - 1) * 30 * DAY)
  let p = jalaliParts(g)
  const target = jy * 13 + jm
  let guard = 0
  while (p.year * 13 + p.month < target && guard++ < 500) { g = new Date(g.getTime() + DAY); p = jalaliParts(g) }
  guard = 0
  while ((p.day !== 1 || p.year * 13 + p.month > target) && guard++ < 40) { g = new Date(g.getTime() - DAY); p = jalaliParts(g) }
  return new Date(Date.UTC(g.getUTCFullYear(), g.getUTCMonth(), g.getUTCDate()))
}
function iso(d) { return d.toISOString().slice(0, 10) }

// 1) parts for a spread of UTC instants (Tehran wall-clock conversion boundaries).
const instants = [
  '2023-03-20T12:00:00Z', '2023-03-21T00:00:00Z', // Nowruz 1402 boundary
  '2024-03-19T21:00:00Z', '2024-03-20T00:00:00Z', '2024-03-20T12:00:00Z', // Nowruz 1403
  '2025-03-20T12:00:00Z', // Nowruz 1404
  '2025-01-01T00:00:00Z', '2025-06-15T08:30:00Z', '2025-09-23T18:00:00Z',
  '2024-02-29T12:00:00Z', // leap Gregorian day
  '2025-03-21T12:00:00Z', // after Nowruz
  '2024-12-31T23:59:00Z', '2025-01-01T03:30:00Z',
  '2022-09-21T20:00:00Z', '2022-03-21T12:00:00Z', // pre-2023 Iran DST-era instants
]
const parts = instants.map(s => { const d = new Date(s); return { gregorianUtcIso: s, tehranWall: new Intl.DateTimeFormat('en-GB', { timeZone: 'Asia/Tehran', year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit', hourCycle: 'h23' }).format(d), ...{}, jalali: jalaliParts(d) } })

// 2) month starts for all 12 months of several Jalali years (incl. a leap Esfand).
const years = [1400, 1401, 1402, 1403, 1404]
const monthStarts = []
for (const jy of years) for (let jm = 1; jm <= 12; jm++) {
  monthStarts.push({ jy, jm, gregorianStartUtc: iso(jalaliMonthStart(jy, jm)) })
}

console.log(JSON.stringify({ note: 'Generated from Intl en-u-ca-persian Asia/Tehran; arbiter for the .NET Jalali port.', parts, monthStarts }, null, 2))
