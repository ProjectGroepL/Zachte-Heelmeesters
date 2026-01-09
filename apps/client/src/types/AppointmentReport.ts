export interface AppointmentReportItem {
  description: string
  cost: number
}

export interface AppointmentReport {
  id: number
  summary: string
  totalCost: number
  createdAt: string
  items: AppointmentReportItem[]
}
