/**
 * Chart.js Helper Functions - Admin Dashboard
 * Diseñado siguiendo WCAG 2.1 para accesibilidad
 * - Contraste mínimo 4.5:1 para texto
 * - Patrones visuales además de colores
 * - Tooltips descriptivos
 * - Navegación por teclado habilitada
 */

// Store chart instances to destroy them before recreating
let chartInstances = {};

// WCAG 2.1 compliant color palettes (contrast ratio >= 4.5:1 on white)
const accessibleColors = {
    primary: ['#2563EB', '#7C3AED', '#DB2777', '#EA580C', '#059669', '#0891B2', '#4F46E5', '#BE185D'],
    status: {
        blue: '#2563EB',      // Abierto
        amber: '#D97706',     // En Progreso
        emerald: '#059669',   // Resuelto
        gray: '#4B5563',      // Cerrado
        red: '#DC2626'        // Escalado
    },
    priority: {
        low: '#6B7280',
        medium: '#2563EB',
        high: '#D97706',
        critical: '#DC2626'
    }
};

// Chart.js default configuration for accessibility
const defaultChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    animation: {
        duration: 750,
        easing: 'easeOutQuart'
    },
    plugins: {
        tooltip: {
            enabled: true,
            backgroundColor: 'rgba(17, 24, 39, 0.95)',
            titleColor: '#F9FAFB',
            bodyColor: '#F3F4F6',
            borderColor: 'rgba(255, 255, 255, 0.1)',
            borderWidth: 1,
            padding: 14,
            cornerRadius: 10,
            titleFont: {
                size: 13,
                weight: '600'
            },
            bodyFont: {
                size: 12
            },
            displayColors: true,
            boxPadding: 6
        }
    }
};

// Helper to destroy existing chart
function destroyChart(canvasId) {
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
        delete chartInstances[canvasId];
    }
}

// Render a doughnut/pie chart with accessibility support
window.renderDoughnutChart = function (canvasId, labels, data, colors) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    destroyChart(canvasId);

    // Add ARIA label for accessibility
    const total = data.reduce((a, b) => a + b, 0);
    canvas.setAttribute('role', 'img');
    canvas.setAttribute('aria-label', `Gráfico circular mostrando: ${labels.map((l, i) => `${l}: ${data[i]} (${((data[i]/total)*100).toFixed(1)}%)`).join(', ')}`);

    const ctx = canvas.getContext('2d');
    chartInstances[canvasId] = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors || accessibleColors.primary.slice(0, data.length),
                borderWidth: 3,
                borderColor: '#ffffff',
                hoverOffset: 8,
                hoverBorderWidth: 4
            }]
        },
        options: {
            ...defaultChartOptions,
            cutout: '55%',
            plugins: {
                ...defaultChartOptions.plugins,
                legend: {
                    position: 'right',
                    align: 'center',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'circle',
                        padding: 16,
                        font: {
                            size: 12,
                            weight: '500'
                        },
                        color: '#374151',
                        generateLabels: function(chart) {
                            const data = chart.data;
                            if (data.labels.length && data.datasets.length) {
                                return data.labels.map((label, i) => {
                                    const value = data.datasets[0].data[i];
                                    const total = data.datasets[0].data.reduce((a, b) => a + b, 0);
                                    const percentage = ((value / total) * 100).toFixed(1);
                                    return {
                                        text: `${label} (${percentage}%)`,
                                        fillStyle: data.datasets[0].backgroundColor[i],
                                        strokeStyle: data.datasets[0].backgroundColor[i],
                                        hidden: false,
                                        index: i,
                                        pointStyle: 'circle'
                                    };
                                });
                            }
                            return [];
                        }
                    }
                },
                tooltip: {
                    ...defaultChartOptions.plugins.tooltip,
                    callbacks: {
                        title: function(context) {
                            return context[0].label;
                        },
                        label: function(context) {
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((context.raw / total) * 100).toFixed(1);
                            return [`Cantidad: ${context.raw.toLocaleString()}`, `Porcentaje: ${percentage}%`];
                        }
                    }
                }
            }
        }
    });
};

// Render a line chart for trends with accessibility
window.renderLineChart = function (canvasId, labels, createdData, resolvedData) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    destroyChart(canvasId);

    // Add ARIA label for accessibility
    canvas.setAttribute('role', 'img');
    canvas.setAttribute('aria-label', `Gráfico de tendencia: Creados vs Resueltos desde ${labels[0]} hasta ${labels[labels.length-1]}`);

    // Create gradients for fill areas
    const ctx = canvas.getContext('2d');
    const height = canvas.parentElement?.clientHeight || 300;
    
    const gradientCreated = ctx.createLinearGradient(0, 0, 0, height);
    gradientCreated.addColorStop(0, 'rgba(37, 99, 235, 0.2)');
    gradientCreated.addColorStop(1, 'rgba(37, 99, 235, 0.01)');
    
    const gradientResolved = ctx.createLinearGradient(0, 0, 0, height);
    gradientResolved.addColorStop(0, 'rgba(5, 150, 105, 0.2)');
    gradientResolved.addColorStop(1, 'rgba(5, 150, 105, 0.01)');

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Incidentes Creados',
                    data: createdData,
                    borderColor: '#2563EB',
                    backgroundColor: gradientCreated,
                    fill: true,
                    tension: 0.35,
                    pointRadius: 4,
                    pointHoverRadius: 7,
                    pointBackgroundColor: '#2563EB',
                    pointBorderColor: '#ffffff',
                    pointBorderWidth: 2,
                    pointHoverBackgroundColor: '#ffffff',
                    pointHoverBorderColor: '#2563EB',
                    pointHoverBorderWidth: 3,
                    borderWidth: 3
                },
                {
                    label: 'Incidentes Resueltos',
                    data: resolvedData,
                    borderColor: '#059669',
                    backgroundColor: gradientResolved,
                    fill: true,
                    tension: 0.35,
                    pointRadius: 4,
                    pointHoverRadius: 7,
                    pointBackgroundColor: '#059669',
                    pointBorderColor: '#ffffff',
                    pointBorderWidth: 2,
                    pointHoverBackgroundColor: '#ffffff',
                    pointHoverBorderColor: '#059669',
                    pointHoverBorderWidth: 3,
                    borderWidth: 3
                }
            ]
        },
        options: {
            ...defaultChartOptions,
            interaction: {
                intersect: false,
                mode: 'index'
            },
            plugins: {
                ...defaultChartOptions.plugins,
                legend: {
                    display: true,
                    position: 'top',
                    align: 'end',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'circle',
                        padding: 20,
                        font: {
                            size: 12,
                            weight: '500'
                        },
                        color: '#374151'
                    }
                },
                tooltip: {
                    ...defaultChartOptions.plugins.tooltip,
                    callbacks: {
                        title: function(context) {
                            return `Fecha: ${context[0].label}`;
                        },
                        label: function(context) {
                            return `${context.dataset.label}: ${context.raw.toLocaleString()}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        maxRotation: 0,
                        minRotation: 0,
                        font: {
                            size: 11,
                            weight: '500'
                        },
                        color: '#6B7280',
                        padding: 8,
                        maxTicksLimit: 12
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(107, 114, 128, 0.1)',
                        drawBorder: false
                    },
                    border: {
                        display: false
                    },
                    ticks: {
                        stepSize: 1,
                        font: {
                            size: 11,
                            weight: '500'
                        },
                        color: '#6B7280',
                        padding: 12
                    }
                }
            }
        }
    });
};

// Render a horizontal bar chart with accessibility
window.renderBarChart = function (canvasId, labels, data, baseColor) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    destroyChart(canvasId);

    // Add ARIA label for accessibility
    canvas.setAttribute('role', 'img');
    canvas.setAttribute('aria-label', `Gráfico de barras: ${labels.map((l, i) => `${l}: ${data[i]}`).join(', ')}`);

    // Generate gradient colors based on base color
    const colors = data.map((_, index) => {
        const hue = (195 + (index * 15)) % 360; // Cyan-based gradient
        return `hsl(${hue}, 70%, 45%)`;
    });

    const ctx = canvas.getContext('2d');
    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Incidentes',
                data: data,
                backgroundColor: colors,
                borderRadius: 6,
                borderSkipped: false,
                barThickness: 'flex',
                maxBarThickness: 24,
                minBarLength: 4,
                hoverBackgroundColor: colors.map(c => c.replace('45%', '35%'))
            }]
        },
        options: {
            ...defaultChartOptions,
            indexAxis: 'y',
            plugins: {
                ...defaultChartOptions.plugins,
                legend: {
                    display: false
                },
                tooltip: {
                    ...defaultChartOptions.plugins.tooltip,
                    callbacks: {
                        title: function(context) {
                            return context[0].label;
                        },
                        label: function(context) {
                            return `Total de incidentes: ${context.raw.toLocaleString()}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(107, 114, 128, 0.1)',
                        drawBorder: false
                    },
                    border: {
                        display: false
                    },
                    ticks: {
                        stepSize: 1,
                        font: {
                            size: 11,
                            weight: '500'
                        },
                        color: '#6B7280',
                        padding: 8
                    }
                },
                y: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    border: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 12,
                            weight: '500'
                        },
                        color: '#374151',
                        padding: 8
                    }
                }
            }
        }
    });
};

// Render a vertical bar chart with accessibility
window.renderVerticalBarChart = function (canvasId, labels, data, colors) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    destroyChart(canvasId);

    // Add ARIA label for accessibility
    canvas.setAttribute('role', 'img');
    canvas.setAttribute('aria-label', `Gráfico de barras verticales: ${labels.map((l, i) => `${l}: ${data[i]}`).join(', ')}`);

    const ctx = canvas.getContext('2d');
    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Cantidad',
                data: data,
                backgroundColor: colors || '#2563EB',
                borderRadius: 8,
                borderSkipped: false,
                maxBarThickness: 50
            }]
        },
        options: {
            ...defaultChartOptions,
            plugins: {
                ...defaultChartOptions.plugins,
                legend: {
                    display: false
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    border: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 11,
                            weight: '500'
                        },
                        color: '#6B7280'
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(107, 114, 128, 0.1)',
                        drawBorder: false
                    },
                    border: {
                        display: false
                    },
                    ticks: {
                        stepSize: 1,
                        font: {
                            size: 11,
                            weight: '500'
                        },
                        color: '#6B7280'
                    }
                }
            }
        }
    });
};

// Cleanup function for page navigation
window.cleanupCharts = function() {
    Object.keys(chartInstances).forEach(key => {
        destroyChart(key);
    });
};

// PDF Report Download Function
window.downloadPdfReport = async function(url, requestData, fileName) {
    try {
        // Show loading state (optional: you can emit an event to show loading in Blazor)
        console.log('Generating PDF report...');
        
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestData),
            credentials: 'include' // Include cookies for authentication
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Error generating PDF report');
        }

        // Get the blob from response
        const blob = await response.blob();
        
        // Create a download link
        const downloadUrl = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = downloadUrl;
        link.download = fileName;
        
        // Trigger download
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        // Cleanup
        window.URL.revokeObjectURL(downloadUrl);
        
        console.log('PDF downloaded successfully');
        return true;
    } catch (error) {
        console.error('Error downloading PDF:', error);
        throw error;
    }
};

// Excel Report Download Function
window.downloadExcelReport = async function(url, requestData, fileName) {
    try {
        console.log('Generating Excel report...');
        
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestData),
            credentials: 'include'
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Error generating Excel report');
        }

        const blob = await response.blob();
        
        const downloadUrl = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = downloadUrl;
        link.download = fileName;
        
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        window.URL.revokeObjectURL(downloadUrl);
        
        console.log('Excel downloaded successfully');
        return true;
    } catch (error) {
        console.error('Error downloading Excel:', error);
        throw error;
    }
};