// Course model interface
export interface Course {
  id: string;
  name: string;
  instructor: string;
  description: string;
  category: string;
  duration: string;
  level: 'Beginner' | 'Intermediate' | 'Advanced';
  icon: string;
}
