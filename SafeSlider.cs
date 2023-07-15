using System;
using Xamarin.Forms;

namespace Common.Controls
{
    public class SafeSlider : Slider
    {
        private SliderValues reqValues = new SliderValues(0, 0, 1);
        private SliderValues validValues = new SliderValues(0, 0, 1);

        public EventHandler<ValueChangedEventArgs> SafeSliderValueChanged;

        private void UpdateSliderValues(SliderValues newValues, Slider slider)
        {
            double oldMinimum = validValues.Minimum;
            double oldValue = validValues.Value;
            double oldMaximum = validValues.Maximum;

            validValues = newValues;

            double newMinimum = newValues.Minimum;
            double newValue = newValues.Value;
            double newMaximum = newValues.Maximum;

            void Set1()
            {
                slider.Minimum = newMinimum;
            }

            void Set2()
            {
                slider.Value = newValue;
            }

            void Set3()
            {
                slider.Maximum = newMaximum;
            }

            UnregisterValueChangeEvent(slider);

            if (newMaximum >= oldMaximum && newValue >= oldValue)
            {
                Set3();
                Set2();
                Set1();
            }
            else if (newMaximum >= oldMaximum && newValue < oldValue)
            {
                Set3();
                Set1();
                Set2();
            }
            else if (newMinimum < oldMinimum && newValue < oldValue)
            {
                Set1();
                Set2();
                Set3();
            }
            else if (newMinimum < oldMinimum && newValue >= oldValue)
            {
                Set1();
                Set3();
                Set2();
            }
            else // n1 >= o1 && n3 < o3
            {
                Set2();
                Set1();
                Set3();
            }

            RegisterValueChangeEvent(slider);
        }

        private void UnregisterValueChangeEvent(Slider slider)
        {
            slider.ValueChanged -= SafeSliderValueChanged;
        }

        private void RegisterValueChangeEvent(Slider slider)
        {
            slider.ValueChanged += SafeSliderValueChanged;
        }

        public double SafeMinimum
        {
            get { return validValues.Minimum; }
            set
            {
                reqValues = new SliderValues(value, reqValues.Value, reqValues.Maximum);
                UpdateSliderValues(reqValues.ForceValid, this);
            }
        }

        public double SafeValue
        {
            get { return validValues.Value; }
            set
            {
                reqValues = new SliderValues(reqValues.Minimum, value, reqValues.Maximum);
                UpdateSliderValues(reqValues.ForceValid, this);
            }
        }

        public double SafeMaximum
        {
            get { return validValues.Maximum; }
            set
            {
                reqValues = new SliderValues(reqValues.Minimum, reqValues.Value, value);
                UpdateSliderValues(reqValues.ForceValid, this);
            }
        }

        public SliderValues SliderValues
        {
            get { return validValues; }
            set
            {
                if (value.IsValid)
                {
                    reqValues = value;
                    UpdateSliderValues(value, this);
                }
                else
                {
                    throw new Exception("Incorrectly ordered slider values");
                }
            }
        }
    }

    public class SliderValues
    {
        public double Minimum { get; }
        public double Value { get; }
        public double Maximum { get; }

        public SliderValues(double minimum, double value, double maximum)
        {
            Minimum = minimum;
            Value = value;
            Maximum = maximum;
        }

        public bool IsValid => Minimum <= Value && Value <= Maximum;

        public SliderValues ForceValid
        {
            get
            {
                if (IsValid)
                    return this;

                return new SliderValues(Math.Min(Minimum, Math.Min(Value, Maximum)),
                                        Math.Max(Minimum, Math.Max(Value, Maximum)),
                                        Math.Max(Minimum, Math.Max(Value, Maximum)));
            }
        }
    }

}
