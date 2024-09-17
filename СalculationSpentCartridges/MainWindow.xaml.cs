using System.Diagnostics;
using System.IO;
using System.Windows;

namespace СalculationSpentCartridges
{
    public partial class MainWindow : Window
    {
        private int totalIssued;
        private int lostSleevesCount;
        private int numberOfSoldiers;
        private int patronPerExercise;
        private string statementName = "Ведомость.csv";
        private bool append = false;

        public MainWindow()
        {
            InitializeComponent();
            SurnameStackPanel.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            lostSleevesCount = int.Parse(LostSleeves.Text);

            int returnsSleevesCount = totalIssued - lostSleevesCount;
            int endRandom = returnsSleevesCount / numberOfSoldiers;
            int startRandom = (endRandom - 25 > 0) ? endRandom - 25 : 0;
            List<int> returnsSleevesList = new();
            List<int> lostSleevesList = new();
            int number = default;
            int completion;

            for (int i = 0; i < numberOfSoldiers; i++)
            {
                number = r.Next(startRandom, endRandom);
                returnsSleevesList.Add(number);
            }

            completion = returnsSleevesCount - returnsSleevesList.Sum();

            for (int i = 0; completion != 0; i++, completion--)
            {
                if (i > returnsSleevesList.Count - 1)
                    i = 0;
                if (returnsSleevesList[i] >= patronPerExercise)
                {
                    completion++;
                    continue;
                }
                returnsSleevesList[i]++;
            }
            returnsSleevesList.ForEach(x => lostSleevesList.Add(patronPerExercise - x));

            CreateAndOpenFile(returnsSleevesList, lostSleevesList);
        }

        private void CreateAndOpenFile(List<int> sleevesList, List<int> lostsleeves)
        {
            if (SurnameStackPanel.Visibility == Visibility.Visible)
                statementName = String.Concat(SurnameTextBox.Text, ".csv");
            try
            {
                using (StreamWriter sr = new StreamWriter(Directory.GetCurrentDirectory() + "\\" + statementName, append, System.Text.Encoding.Unicode))
                {
                    sr.Write($"\t{numberOfSoldiers} человек по {patronPerExercise}\n");
                    sr.Write("№ \t Возвращено \t Утеряно\n");
                    for (int i = 0; i < sleevesList.Count; i++)
                    {
                        sr.WriteLine($"{i + 1} \t" + sleevesList[i] + "\t" + lostsleeves[i]);
                    }
                }
                Process.Start("explorer.exe", Directory.GetCurrentDirectory() + "\\" + statementName);
            }
            catch (Exception)
            {
                MessageBox.Show($"вероятно открыт файл {statementName} - закройте его и повторите расчет");
            }
        }

        private void UpdateTextBlock(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(NumberOfSoldiers.Text, out numberOfSoldiers) && int.TryParse(PatronPerExercise.Text, out patronPerExercise))
            {
                totalIssued = numberOfSoldiers * patronPerExercise;
                TotalIssued.Text = totalIssued.ToString();
                StackPanel.UpdateLayout();
            }
        }

        private void rdBtn_Checked(object sender, RoutedEventArgs e)
        {
            SurnameStackPanel.Visibility = Visibility.Visible;
            append = true;
        }
    }
}