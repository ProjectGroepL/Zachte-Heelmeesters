export interface InsuranceInvoiceDto {
  invoiceId: number
  amount: number
  date: string
}

export interface CreateInsuranceInvoiceDto {
  appointmentReportId: number
  insurerId: number
}
export interface SetCoveredAmountDto {
  coveredAmount: number
}
export interface InsuranceInvoiceFullDto {
  invoiceId: number
  amount: number
  coveredAmount: number 
  patientPays: number
  date: string
}
export interface InsuranceInvoiceForInsurer {
  invoiceId: number
  amount: number
  coveredAmount: number | null
  patientPays: number
  date: string
}