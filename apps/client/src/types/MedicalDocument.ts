export type MedicalDocumentStatus =
  | 'Draft'
  | 'Final'
  | 'Archived'

export interface MedicalDocument {
  id: number
  title: string
  content: string
  status: MedicalDocumentStatus
  createdAt: string
  createdBy: string
}