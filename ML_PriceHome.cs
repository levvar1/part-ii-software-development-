using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML;
namespace ML_PriceHome
{
    //описываем структуру данных, входные данные 
    public class HouseDate
    {
        [LoadColumn(0)] public float Size { get; set; }//площадь в кв2
        [LoadColumn(1)] public float Price { get; set; }//цена 
    }
    //описываем предсказания(итог)
    public class Prediction
    {
        [ColumnName("Score")] public float Price { get; set; }//цена из предсказания
    }

    internal class ML_PriceHome
    {
        static void Main(string[] args)
        {
            var context=new MLContext();//глав объект ML.net
            var data = new[]
            {
                new HouseDate{Size=30,Price=3.0f },
                new HouseDate{Size=45,Price=4.5f },
                new HouseDate{Size=60,Price=6.0f },
                new HouseDate{Size=80,Price=8.0f },
                new HouseDate{Size=100,Price=10.0f }
            };
            //загружаем данные в память 
            var trainingDate=context.Data.LoadFromEnumerable(data);
            //обучение 
            var pipeline = context.Transforms.Concatenate("Features", "Size").Append(context.Regression.Trainers.Sdca(labelColumnName:"Price",maximumNumberOfIterations:100));

            var model= pipeline.Fit(trainingDate);
            //запуск обучения 
            //интеректив с пользователем 
            Console.WriteLine("---Нейросеть риэлтор жилья---");
            while (true)
            {
                Console.WriteLine("\n Введите площадь квартиры(кв.м)");
                string input=Console.ReadLine();
                if (float.TryParse(input, out float userSize) && userSize > 0)
                {
                    //предсказание на основе обученной модели 
                    var predictionEngine=context.Model.CreatePredictionEngine<HouseDate,Prediction>(model);
                    //прогноз введенного числа
                    var result=predictionEngine.Predict(new HouseDate { Size=userSize });
                    Console.WriteLine($"Прогноз {result.Price:F2}");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
