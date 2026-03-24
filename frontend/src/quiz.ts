// Quiz model interfaces
export interface QuizQuestion {
  question: string;
  options: string[];
  correctIndex: number;
}

export interface QuizResult {
  courseId: string;
  courseName: string;
  score: number;
  total: number;
  percentage: number;
  date: string;
  passed: boolean;
}

export interface CourseQuiz {
  courseId: string;
  questions: QuizQuestion[];
}
