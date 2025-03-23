using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;

namespace BusDataConvert
{
    public partial class Form1 : Form
    {
        List<FileData> fdList = new();

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "�뼱 ���� ���� (*.txt, *.xlsx)|*.txt;*.xlsx"; ;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (fdList.Any(p => p.fileName == ofd.SafeFileName) == false)
                {
                    fdList.Add(new FileData(ofd.FileName, ofd.SafeFileName));
                }
                else
                {
                    MessageBox.Show("�̹� �߰��� �������Դϴ�.", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                return;
            }

            listBox1.Items.Clear();
            listBox1.Items.AddRange(fdList.Select(p => p.fileName).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //���� ��ġ ����
            SaveFileDialog sfd = new();
            sfd.Filter = "json ���� (*.json)|*.json";
            string savePathName = "";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                savePathName = sfd.FileName;
            }
            else
            {
                return;
            }

            //��ȯ ����
            button2.Text = "��ȯ ��";
            button2.BackColor = System.Drawing.Color.FromArgb(240, 128, 128);
            button1.Enabled = false;
            button2.Enabled = false;

            List<BusData> busDataList = new();

            foreach (FileData fd in fdList)
            {
                Match fileShape = Regex.Match(fd.fileName, @"\.[^\.]+$");

                //��⵵ ���� �������� ��� (txt ����)
                if (fileShape.Value == ".txt")
                {
                    string rawData = File.ReadAllText(fd.filePath);

                    List<string> dataLineSplit = new();
                    dataLineSplit.AddRange(Regex.Split(rawData, "\\^"));

                    for (int i = 1; i < dataLineSplit.Count; i++)
                    {
                        List<string> dataVBSplit = new();
                        dataVBSplit.AddRange(Regex.Split(dataLineSplit[i], "\\|"));

                        //���� ��Ʈ�� ó������ �ִ� ���
                        if (busDataList.Any(p => p.routeId == dataVBSplit[0]) == false)
                        {
                            BusData bd = new BusData
                            {
                                routeId = dataVBSplit[0],
                                routeName = dataVBSplit[1],
                                stationDataList =
                                {
                                    new StationData { stationId = dataVBSplit[4], stationName = dataVBSplit[5], posX = dataVBSplit[6], posY = dataVBSplit[7] }
                                }
                            };

                            busDataList.Add(bd);
                        }

                        //�̹� �����ϴ� ���� ��Ʈ�� ���� ���
                        else
                        {
                            busDataList.FirstOrDefault(p => p.routeId == dataVBSplit[0]).stationDataList.Add(new StationData { stationId = dataVBSplit[4], stationName = dataVBSplit[5], posX = dataVBSplit[6], posY = dataVBSplit[7] });
                        }
                    }
                }

                //���� ���� �������� ��� (.xlsx)
                if (fileShape.Value == ".xlsx")
                {
                    using (var workbook = new XLWorkbook(fd.filePath))
                    {
                        var worksheet = workbook.Worksheet(1);
                        int lastRow = worksheet.LastRowUsed().RowNumber();

                        for (int row = 2; row <= lastRow; row++)
                        {
                            var rowData = worksheet.Row(row);

                            //���� ��Ʈ�� ó������ �ִ� ���
                            if (busDataList.Any(p => p.routeId == rowData.Cell(1).GetValue<string>()) == false)
                            {
                                BusData bd = new BusData
                                {
                                    routeId = rowData.Cell(1).GetValue<string>(),
                                    routeName = rowData.Cell(2).GetValue<string>(),
                                    stationDataList =
                                    {
                                        new StationData { stationId = rowData.Cell(4).GetValue<string>(), stationName = rowData.Cell(6).GetValue<string>(), posX = rowData.Cell(7).GetValue<string>(), posY = rowData.Cell(8).GetValue<string>() }
                                    }
                                };

                                busDataList.Add(bd);
                            }

                            //�̹� �����ϴ� ���� ��Ʈ�� ���� ���
                            else
                            {
                                busDataList.FirstOrDefault(p => p.routeId == rowData.Cell(1).GetValue<string>()).stationDataList.Add(new StationData { stationId = rowData.Cell(4).GetValue<string>(), stationName = rowData.Cell(6).GetValue<string>(), posX = rowData.Cell(7).GetValue<string>(), posY = rowData.Cell(8).GetValue<string>() });
                            }
                        }
                    }
                }
            }

            string json = JsonConvert.SerializeObject(busDataList, Formatting.Indented);
            File.WriteAllText(savePathName, json);

            MessageBox.Show("������ �Ϸ�Ǿ����ϴ�.", "�˸�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            button2.Text = "��ȯ ����";
            button2.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            button1.Enabled = true;
            button2.Enabled = true;
        }
    }

    class FileData(string filePath, string fileName)
    {
        public string filePath = filePath;
        public string fileName = fileName;
    }

    class BusData
    {
        [JsonProperty(Order = 1)]
        public string routeId { get; set; }

        [JsonProperty(Order = 2)]
        public string routeName { get; set; }

        [JsonProperty(Order = 3)]
        public List<StationData> stationDataList = new();
    }

    class StationData
    {
        public string stationId { get; set; }
        public string stationName { get; set; }
        public string posX { get; set; }
        public string posY { get; set; }
    }
}