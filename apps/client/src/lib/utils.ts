import type { User } from "@/types/Auth"
import type { ClassValue } from "clsx"
import { clsx } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function getFullNameFromUser(user: User): string {
  return `${user.firstName} ${user.middleName ? user.middleName + " " : ""}${user.lastName}`
}