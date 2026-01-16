export interface ReportSummaryDto {
  appointmentId: number
  summary: string
}
export interface PatientReportDto {
  id: number
  summary: string
  createdAt: string
  items: string[]
}