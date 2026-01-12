export type NotificationType =
  | 'AccessRequest'
  | 'AccessRequestDecision'
  | 'AppointmentReport'
  | 'General'

export interface Notification {
  id: number
  message: string
  isRead: boolean
  type: NotificationType
  accessRequestId?: number
  createdAt: string
}
