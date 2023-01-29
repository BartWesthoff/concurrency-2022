using System;
using Quiz;

// todo: Complete the implementation

/// Concurrent version of the Quiz
namespace ConcQuiz
{
    public class ConcAnswer : Answer
    {
        public ConcAnswer(ConcStudent std, string txt = "") : base(std, txt) { }
    }

    public class ConcQuestion : Question
    {
        //todo: add required fields, if necessary
        private static readonly Mutex mutex = new Mutex();

        public ConcQuestion(string txt, string tcode) : base(txt, tcode) { }

        public override void AddAnswer(Answer a)
        {
            //todo: implement the body 
            lock (mutex)
            {
                this.Answers.AddLast(a);
            }
        }
    }

    public class ConcStudent : Student
    {
        // todo: add required fields

        public ConcStudent(int num, string name) : base(num, name) { }

        public override void AssignExam(Exam e)
        {
            //todo: implement the body
            base.AssignExam(e);
        }

        public override void StartExam()
        {
            //todo: implement the body
            base.StartExam();
        }

        public override void Think()
        {
            //todo: implement the body
            base.Think();
        }

        public override void ProposeAnswer()
        {
            //todo: implement the body
            base.ProposeAnswer();
        }

        public override void Log(string logText = "")
        {
            base.Log();
        }

    }
    public class ConcTeacher : Teacher
    {
        //todo: add required fields, if necessary

        public ConcTeacher(string code, string name) : base(code, name) { }

        public override void AssignExam(Exam e)
        {
            //todo: implement the body
            base.AssignExam(e);
        }
        public override void Think()
        {
            //todo: implement the body
            base.Think();
        }
        public override void ProposeQuestion()
        {
            //todo: implement the body
            base.ProposeQuestion();
        }
        public override void PrepareExam(int maxNumOfQuestions)
        {
            //todo: implement the body
            base.PrepareExam(maxNumOfQuestions);
        }
        public override void Log(string logText = "")
        {
            base.Log();
        }
    }
    public class ConcExam : Exam
    {
        //todo: add required fields, if necessary
        private static readonly Mutex mutex = new();

        private int QuestionNumber;

        public ConcExam(int number, string name = "") : base(number, name) {}

        public override void AddQuestion(Teacher teacher, string text)
        {
            lock (mutex)
            {
                ConcQuestion question = new ConcQuestion(text, teacher.Code);
                this.Questions.AddLast(question);
                this.QuestionNumber++;

                this.Log($"[Question is added] {question}");
            }
        }

        public override void Log(string logText = "")
        {
            base.Log();
        }

        public override string ToString()
        {
            return $"Exam : 0 : Total Num Questions: {this.QuestionNumber}\n";
        }
    }

    public class ConcClassroom : Classroom
    {
        //todo: add required fields, if necessary
        public int distributedExams = 0;

        public ConcClassroom(int examNumber = 1, string examName = "Programming") : base(examNumber, examName)
        {
            //todo: implement the body
            this.Exam = new ConcExam(examNumber, examName);
        }

        public override void SetUp()
        {
            //todo: implement the body
            for (int i = 0; i < FixedParams.maxNumOfStudents; i++)
            {
                string std_name = "STUDENT NAME"; //may be genrated later, or not
                this.Students.AddLast(new ConcStudent(i + 1, std_name));
            }
            for (int i = 0; i < FixedParams.maxNumOfTeachers; i++)
            {
                string teacher_name = "TEACHER NAME"; //may be genrated later, or not
                this.Teachers.AddLast(new ConcTeacher((i + 1).ToString(), teacher_name));
            }
            // assign exams
            foreach (ConcTeacher t in this.Teachers)
                t.AssignExam(this.Exam);
        }

        public override void PrepareExam(int maxNumOfQuestion)
        {
            //todo: implement the body
            // NOTE: Moet wel goed zijn dit
            List<Thread> threads = new List<Thread>();
           
            //foreach (ConcTeacher t in this.Teachers)
            //{
            //    Thread tr = new Thread(() => t.PrepareExam(maxNumOfQuestion));
            //    threads.Add(tr);
            //    tr.Start();
            //}
            //foreach (Thread thr in threads)
            //{
            //    thr.Join();
            //}

            foreach (var teacher in Teachers)
            {
                threads.Add(new Thread(() => teacher.PrepareExam(maxNumOfQuestion)));
            };
            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
        public override void DistributeExam()
        {
            //todo: implement the body
            // NOTE: Moet wel goed zijn dit
            foreach (ConcStudent s in this.Students)
            {
                s.AssignExam(this.Exam);
            }
        }
        public override void StartExams()
        {
            //todo: implement the body
            // NOTE: Moet wel goed zijn dit
            List<Thread> threads = new List<Thread>();
            foreach (var student in this.Students)
            {
                threads.Add(new Thread(() => student.StartExam()));
            };
            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }


    }

    //THIS CLASS (QUIZCONCURRENT) SHOULD NOT BE CHANGED
    public class QuizConcurrent
    {
        ConcClassroom classroom;

        public QuizConcurrent()
        {
            this.classroom = new ConcClassroom();
        }
        public void RunExams()
        {
            classroom.SetUp();
            classroom.PrepareExam(Quiz.FixedParams.maxNumOfQuestions);
            classroom.DistributeExam();
            classroom.StartExams();
        }
        public string FinalResult()
        {
            return classroom.GetStatistics();
        }

    }
}
