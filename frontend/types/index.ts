// Type definitions for Optiviera SaaS

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  fullName: string
  tenantId: number
  roles: string[]
  createdAt: string
}

export interface AuthResponse {
  token: string
  user: User
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterData {
  companyName: string
  email: string
  password: string
  firstName: string
  lastName: string
}

export interface Ticket {
  id: number
  title: string
  description: string
  status: TicketStatus
  priority: Priority | null
  customerFirstName: string
  customerLastName: string
  customerPhone: string
  customerAddress: string
  customerCity: string
  customerState: string
  customerZip: string
  created: string
  schedule: string
  isArchived: boolean
  technicianName: string | null
  supportName: string | null
}

export interface TicketDetail extends Ticket {
  comments: Comment[]
}

export interface Priority {
  id: number
  name: string
}

export interface Comment {
  id: number
  note: string
  created: string
  userName: string
  userId: string
}

export enum TicketStatus {
  Open = 1,
  InProgress = 2,
  Completed = 3,
  Closed = 4
}

export interface CreateTicketRequest {
  title: string
  description: string
  customerFirstName: string
  customerLastName: string
  customerPhone: string
  customerAddress: string
  customerCity: string
  customerState: string
  customerZip: string
  status?: TicketStatus
  priorityId: number
  technicianId?: string
  supportId?: string
  schedule?: string
}

export interface UpdateTicketRequest {
  title?: string
  description?: string
  status?: TicketStatus
  priorityId?: number
  technicianId?: string
  supportId?: string
  schedule?: string
  isArchived?: boolean
}
