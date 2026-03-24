// Student model interface
export interface Student {
  id: string;
  name: string;
  email: string;
  passwordHash: string;
  enrolledCourseIds: string[];
  avatar?: string;
  joinedDate: string;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  name: string;
  email: string;
  password: string;
}
